using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using EM.Logging;

namespace EM.Api.Core.Models.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode HttpStatus { get; set; }
        
        public ApiException() : base()
        {
            HttpStatus = HttpStatusCode.BadRequest;
        }

        public ApiException(string msg, HttpStatusCode httpStatus = HttpStatusCode.BadRequest): base(msg)
        {
            HttpStatus = httpStatus;
        }

        public ApiException(string msg, Exception e, HttpStatusCode httpStatus = HttpStatusCode.BadRequest): base(msg, e)
        {
            HttpStatus = httpStatus;
            //Logger.getErrorDetails(e);            
        }

        public virtual string FriendlyName
        {
            get { return GetType().Name; }
        }
        
    }
}