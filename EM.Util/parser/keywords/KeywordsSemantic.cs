using System;
using System.Collections.Generic;
using System.Text;

namespace EM.parser.keywords
{
    public abstract class KeywordsSemantic: IKeywordsSemantic
    {
        private IOperator _AND;
        private IOperator _OR;
        private IOperator _NOT;
        private Token.TokenEvaluatorFunction _tokenEvaluator;

        public IOperator AND
        {
            get { return _AND; }
            set { _AND = value; }
        }

        public IOperator OR
        {
            get { return _OR; }
            set { _OR = value; }
        }

        public IOperator NOT
        {
            get { return _NOT; }
            set { _NOT = value; }
        }

        public Token.TokenEvaluatorFunction tokenEvaluator
        {
            get { return _tokenEvaluator; }
            set { _tokenEvaluator = value; }
        }
    }
}
