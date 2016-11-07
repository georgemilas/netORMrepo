using System;
using System.Collections.Generic;
using System.Text;

namespace EM.parser
{
    public class EvaluationException : Exception, IComparable
    {
        public EvaluationException(string msg)
            : base(msg)
        { }
        public EvaluationException(string msg, Exception e)
            : base(msg, e)
        { }

        public virtual int CompareTo(object other)
        {
            return Object.ReferenceEquals(this, other) ? 0 : 1;
        }
    }
}