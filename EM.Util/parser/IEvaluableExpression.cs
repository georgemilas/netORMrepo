using System;
using System.Collections.Generic;
using System.Text;

namespace EM.parser
{
    public interface IEvaluableExpression
    {
        /// <summary>
        /// evaluate the current expression tree against the given text
        /// </summary>
        object evaluate(object obj);   
    }
}
