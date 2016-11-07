using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.TextSearch
{
    public class SearchAND : IOperator
    {
        public object evaluate(object obj, IEvaluableExpression exp) { return true; }  //obj is a string text 
        public object evaluate(object obj, EList<IEvaluableExpression> exps)
        {
            foreach (IEvaluableExpression e in exps)
            {
                bool res = (bool)e.evaluate(obj);
                if (!res)
                {
                    return false;
                }
            }
            return true;
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
