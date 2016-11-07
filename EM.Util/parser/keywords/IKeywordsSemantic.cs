using System;
using System.Collections.Generic;
using System.Text;

namespace EM.parser.keywords
{
    public interface IKeywordsSemantic
    {
        IOperator AND {get; set;}
        IOperator OR { get; set;}
        IOperator NOT { get; set;}
        Token.TokenEvaluatorFunction tokenEvaluator { get; set;}
    }
}
