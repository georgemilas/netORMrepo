using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core
{
    public class ErrorMessage
    {
        public const string UNEXPECTED_EXCEPTION = "UnexpectedException";
        
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public int? LogId { get; set; }
        public HttpStatusCode HttpStatus { get; set; }

        public ErrorMessage() { }  //empty constructor to support XML serialization
        public ErrorMessage(ApiException err) : this(err.HttpStatus, err.Message, err.FriendlyName) { }
        public ErrorMessage(HttpStatusCode status, string message, string errorType = UNEXPECTED_EXCEPTION)
        {
            HttpStatus = status;
            Message = message;
            ErrorType = errorType;
            //LogId = er900Id;
        }
        public IHttpActionResult ToAcctionResult(EMApiController controller)
        {
            return HttpResultTypeNegotiator.GetNegotiator<ErrorMessage>(HttpStatus, this, controller);
        }

        public static IHttpActionResult GetNewErrorAcctionResult(ApiException err, EMApiController controller)
        {
            return GetNewErrorAcctionResult(err.HttpStatus, err.Message, controller, err.FriendlyName);
        }
        public static IHttpActionResult GetNewErrorAcctionResult(HttpStatusCode status, string message, EMApiController controller, string errorType = UNEXPECTED_EXCEPTION)
        {
            var m = new ErrorMessage(status, message, errorType);
            return m.ToAcctionResult(controller);
        }
    }
}