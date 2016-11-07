using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.exceptions
{
    public class SQLSintaxException : ValidationException
    {
        public SQLSintaxException(string msg)
            : base(msg)
        { }
        public SQLSintaxException(string msg, Exception e)
            : base(msg, e)
        { }
    }

}
