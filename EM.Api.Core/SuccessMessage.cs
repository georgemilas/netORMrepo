using System.Net;
using System.Web.Http;

namespace EM.Api.Core
{
    public class SuccessMessage
    {
        public string Message { get; set; }
        public HttpStatusCode HttpStatus { get; set; }

        public SuccessMessage() { }  //empty constructor to support XML serialization
        public SuccessMessage(string message, HttpStatusCode status = HttpStatusCode.OK)
        {
            HttpStatus = status;
            Message = message;            
        }

        public IHttpActionResult ToAcctionResult(EMApiController controller)
        {
            return HttpResultTypeNegotiator.GetNegotiator<SuccessMessage>(HttpStatus, this, controller);
        }
    }
}