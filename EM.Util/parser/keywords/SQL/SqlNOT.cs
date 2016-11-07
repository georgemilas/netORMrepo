using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.SQL
{
    public class SqlNOT : SQLOperator, IOperator
    {
        //obj is always null, as we are not realy evaluating anything but transformin the kewords text into a SQL WHERE clause as string
        public object evaluate(object obj, IEvaluableExpression exp)
        {
            return string.Format("NOT ({0})", exp.evaluate(null));
        }
        public object evaluate(object obj, EList<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> canot negate a list");
        }
        public string ToString(IEvaluableExpression exp)
        {
            return "NOT " + exp.ToString();
        }
        public string ToString(EList<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> canot negate a list");
        }
    }
}
