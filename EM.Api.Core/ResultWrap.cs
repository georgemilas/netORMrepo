using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core
{

    public interface IResultWrap
    {
        IHttpActionResult GetActionResult(EMApiController controller);
    }

    /// <summary>
    /// Wrapps a result with 3 possible values<para></para>
    /// - Some actual Data of type T to be negociated into an 200 ActionResult<para></para>
    /// - An Error as an ActionResult<para></para>
    /// - A custom ActionResult<para></para>
    /// </summary>
    public class ResultWrap<T> : IResultWrap where T : class 
    {
        protected ResultWrap() { }
        private IHttpActionResult ActionResult { get; set; }
        private T DataResult { get; set; }
        private ErrorMessage ErrorResult { get; set; }

        /// <summary>
        /// Unwraps the inner data, error or action into an appropriate IHttpActionResult
        /// </summary>
        public IHttpActionResult GetActionResult(EMApiController controller)
        {            
            if (ActionResult != null) { return ActionResult; }
            if (ErrorResult != null) { return ErrorResult.ToAcctionResult(controller); }
            return HttpResultTypeNegotiator.GetNegotiator(HttpStatusCode.OK, DataResult, controller);                            
        }
        
        /// <summary>
        /// A ResultWrap containing the actual custom ActionResult provided <para></para>
        /// One example would be to return a HttpStatusCode.NoContent action result upon a successful PUT in order to conform to the required idempotency of the PUT method
        /// </summary>
        /// <param name="action">The custom AcctionResult</param>
        /// <returns>A custom action result wrap</returns>
        public static ResultWrap<T> Action(IHttpActionResult action)
        {
            return new ResultWrap<T>() { ErrorResult = null, ActionResult = action, DataResult = null };
        }
        
        /// <summary>
        /// Wraps the object into a succesfull action result (HTTP 200)
        /// </summary>
        /// <param name="data">The object to be wrapped</param>
        /// <returns>A success wrap</returns>
        public static ResultWrap<T> ResultData(T data)
        {
            return new ResultWrap<T>() { ErrorResult = null, ActionResult = null, DataResult = data };
        }

        /// <summary>
        /// Wraps the error into an action result with the provided HttpStatusCode
        /// </summary>
        /// <param name="status">The HTTP error status</param>
        /// <param name="message">An error message</param>
        /// <returns>An error wrap</returns>
        public static ResultWrap<T> ErrorMessage(HttpStatusCode status, string message)
        {
            return ErrorMessage(new ApiException(message, status));
        }
        public static ResultWrap<T> ErrorMessage(ApiException err)
        {
            return new ResultWrap<T>()
            {
                ErrorResult = new ErrorMessage(err),
                DataResult = null, 
                ActionResult = null
            };
        }
         
    }


    /// <summary>
    /// Set of convenience functions into ResultWrap&lt;object> and ResultWrap&lt;IEnumerable&lt;object>>
    /// </summary>
    public class ResultWrap
    {
        protected ResultWrap() { }

        /// <summary>
        /// Convenience method for ResultWrap&lt;object>.ErrorMessage<para></para>
        /// Wraps the error into an action result with the provided HttpStatusCode
        /// </summary>
        public static ResultWrap<object> ApiObjectError(HttpStatusCode status, string message)
        {
            return ApiObjectError(new ApiModelValidationException(message, status));
        }
        public static ResultWrap<object> ApiObjectError(ApiException err)
        {
            return ResultWrap<object>.ErrorMessage(err);
        }

        /// <summary>
        /// Convenience method for ResultWrap&lt;IEnumerable&lt;object>>.ErrorMessage<para></para>
        /// Wraps the error into an action result with the provided HttpStatusCode
        /// </summary>
        public static ResultWrap<IEnumerable<object>> ApiObjectsError(HttpStatusCode status, string message)
        {
            return ApiObjectsError(new ApiModelValidationException(message, status));    
        }
        public static ResultWrap<IEnumerable<object>> ApiObjectsError(ApiException err)
        {
            return ResultWrap<IEnumerable<object>>.ErrorMessage(err);
        }

        /// <summary>
        /// Convenience method for ResultWrap&lt;SuccessMessage>.ResultData<para></para>
        /// Wraps the message into an action result with the provided HttpStatusCode, defaulted to 200
        /// </summary>
        public static ResultWrap<SuccessMessage> ApiObjectSuccess(string msg, HttpStatusCode status = HttpStatusCode.OK)
        {
            return ApiObjectSuccess(new SuccessMessage(msg, status));
        }
        /// <summary>
        /// Convenience method for ResultWrap&lt;SuccessMessage>.ResultData<para></para>
        /// </summary>
        public static ResultWrap<SuccessMessage> ApiObjectSuccess(SuccessMessage msg)
        {
            return ResultWrap<SuccessMessage>.ResultData(msg);
        }


        /// <summary>
        /// Convenience method for ResultWrap&lt;object>.Action<para></para>
        /// A ResultWrap containing the actual custom ActionResult provided <para></para>
        /// One example would be to return a HttpStatusCode.NoContent action result upon a successful PUT in order to conform to the required idempotency of the PUT method
        /// </summary>
        /// <param name="action">The custom AcctionResult</param>
        /// <returns>A custom action result wrap</returns>
        public static ResultWrap<object> Action(IHttpActionResult action)
        {
            return ResultWrap<object>.Action(action);
        }
    }



}