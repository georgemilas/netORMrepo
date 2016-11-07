using System;
using System.Net;

namespace EM.Api.Core.Models.Exceptions
{
    public class ApiParameterParsingException : ApiException
    {
        public ApiParameterParsingException(string msg, HttpStatusCode httpStatus = HttpStatusCode.BadRequest) : base(msg, httpStatus) { }
        public ApiParameterParsingException(string msg, Exception err, HttpStatusCode httpStatus = HttpStatusCode.BadRequest) : base(msg, err, httpStatus) { }
    }
}