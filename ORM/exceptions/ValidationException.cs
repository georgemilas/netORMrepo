using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.exceptions
{
    [Serializable]
    public class ValidationException : ORMException
    {
        public enum ERROR_LOCATION { TABLE, FIELD };
        public ERROR_LOCATION errorLocation { get; set; }
        public string fieldName { get; set; }

        public ValidationException() : base() { }

        public ValidationException(string msg)
            : this(msg, ERROR_LOCATION.FIELD)
        { }
        public ValidationException(string msg, Exception e)
            : this(msg, e, ERROR_LOCATION.FIELD)
        { }
        public ValidationException(string msg, ERROR_LOCATION location)
            : base(msg)
        { this.errorLocation = location; }
        public ValidationException(string msg, Exception e, ERROR_LOCATION location)
            : base(msg, e)
        { this.errorLocation = location; }
    }

}
