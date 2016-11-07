using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.SQL
{
    public class SQLOperator
    {
        protected string get_c(string opr, EList<IEvaluableExpression> exps)
        {
            EList<string> res = new EList<string>();
            foreach (IEvaluableExpression a in exps)
            {
                res.Add((string)a.evaluate(null));                
            }
            if (res.Count > 1)
            {
                return "(" + res.join(opr) + ")";
            }
            return res[0];
        }
    }
   
}
    

