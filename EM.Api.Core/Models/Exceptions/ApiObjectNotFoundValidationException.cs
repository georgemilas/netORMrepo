using System;
using System.Net;
namespace EM.Api.Core.Models.Exceptions
{
    public class ApiObjectNotFoundValidationException : ApiException
    {
        public ApiObjectNotFoundValidationException(string msg, HttpStatusCode httpStatus = HttpStatusCode.NotFound) : base(msg, httpStatus) { }
        public ApiObjectNotFoundValidationException(string msg, Exception err, HttpStatusCode httpStatus = HttpStatusCode.NotFound) : base(msg, err, httpStatus) { }
    }
}