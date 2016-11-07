using System;
using EM.Collections;

namespace EM.parser.ObjectQuery
{
    public class ObjectComparerOperator : IOperator
    {
        public string op;
        //private Func<object, object, bool> comparer;

        public ObjectComparerOperator(string op) //, Func<object, object, bool> comparer)
        {
            this.op = op;
            //this.comparer = comparer;
        }
        public object evaluate(object obj, IEvaluableExpression exp) { throw new Exception("Can't eq on one parameter, need 2"); }  //obj is a string text 
        public object evaluate(object obj, EList<IEvaluableExpression> exps)
        {
            throw new NotImplementedException("Evaluation should go to OperatorExpression and should not come down to IOperator");
            //var left = (bool)exps.First().evaluate(obj);
            //var right = (bool)exps.Skip(1).First().evaluate(obj);
            //return comparer(left, right);
        }
        public string ToString(IEvaluableExpression exp) { return exp.ToString(); }
        public string ToString(EList<IEvaluableExpression> exps) { return exps.join(" " + this.op + " "); }
    }
}