using System;
using System.Net;

namespace EM.Api.Core.Models.Exceptions
{
    public class ApiAuthorizationException : ApiException
    {
        public ApiAuthorizationException(string msg, HttpStatusCode httpStatus = HttpStatusCode.Forbidden) : base(msg, httpStatus) { }
        public ApiAuthorizationException(string msg, Exception err, HttpStatusCode httpStatus = HttpStatusCode.Forbidden) : base(msg, err, httpStatus) { }
    }
}