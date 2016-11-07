using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.exceptions
{
    [Serializable]
    public class ORMException : Exception, IComparable
    {
        public ORMException() : base() { }

        public ORMException(string msg)
            : base(msg)
        { }
        public ORMException(string msg, Exception e)
            : base(msg, e)
        { }

        public virtual int CompareTo(object other)
        {
            return Object.ReferenceEquals(this, other) ? 0 : 1;
        }
    }
}
