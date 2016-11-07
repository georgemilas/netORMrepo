using System;
using EM.Collections;

namespace EM.parser
{
    public interface IOperator
    {
        //static bool _AND(string text, EUtil.Collections.EList<IEvaluableExpression> args);
        //static bool _NOT(string text, IEvaluableExpression arg);
        //static bool _OR(string text, EUtil.Collections.EList<IEvaluableExpression> args);

        object evaluate(object obj, IEvaluableExpression exp);
        object evaluate(object obj, EList<IEvaluableExpression> exp);
        string ToString(IEvaluableExpression exp);
        string ToString(EList<IEvaluableExpression> exp);
    }
}
