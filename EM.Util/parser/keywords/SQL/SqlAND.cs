using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.SQL
{
    public class SqlAND : SQLOperator, IOperator
    {
        //obj is always null, as we are not realy evaluating anything but transformin the kewords text into a SQL WHERE clause as string
        public object evaluate(object obj, IEvaluableExpression exp) { throw new EvaluationException("IOperator._AND -> canot evaluate one element, needs a list of elements"); }
        public object evaluate(object obj, EList<IEvaluableExpression> exps)
        {
            return get_c(" AND ", exps);
        }
        public string ToString(IEvaluableExpression exp)
        {
            return exp.ToString();
        }
        public string ToString(EList<IEvaluableExpression> exps)
        {
            return "(" + exps.join(" AND ") + ")";
        }

    }
}
