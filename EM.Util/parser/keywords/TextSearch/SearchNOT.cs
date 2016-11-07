using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.TextSearch
{
    public class SearchNOT : IOperator
    {
        public object evaluate(object obj, EList<IEvaluableExpression> exp) //obj is a string text 
        {
            throw new EvaluationException("IOperator._NOT -> canot negate a list");
        }
        public object evaluate(object obj, IEvaluableExpression exp)        //obj is a string text 
        {
            return !(bool)exp.evaluate(obj);
        }
        public string ToString(IEvaluableExpression exp)
        {
            return " NOT " + exp.ToString();
        }
        public string ToString(EList<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> canot negate a list");
        }
    }
}
