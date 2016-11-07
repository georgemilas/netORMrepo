using System;
using System.Net;
namespace EM.Api.Core.Models.Exceptions
{
    public class ApiModelValidationException : ApiException
    {
        public ApiModelValidationException(string msg, HttpStatusCode httpStatus = HttpStatusCode.BadRequest) : base(msg, httpStatus) { }
        public ApiModelValidationException(string msg, Exception err, HttpStatusCode httpStatus = HttpStatusCode.BadRequest) : base(msg, err, httpStatus) { }        
    }
}