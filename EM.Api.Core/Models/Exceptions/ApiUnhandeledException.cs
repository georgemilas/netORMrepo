using System;
using System.Net;

namespace EM.Api.Core.Models.Exceptions
{
    public class ApiUnexpectedException : ApiException
    {
        public ApiUnexpectedException(string msg, HttpStatusCode httpStatus = HttpStatusCode.InternalServerError) : base(msg, httpStatus) { }
        public ApiUnexpectedException(string msg, Exception err, HttpStatusCode httpStatus = HttpStatusCode.InternalServerError) : base(msg, err, httpStatus) { }
    }
}