using System;
using System.Collections.Generic;
using System.Text;

namespace EM.parser
{
    /// <summary>
    /// evaluate a 'token' by setting a 'tokenEvaluator' function so that tokenEvaluator(token) 
    /// represent the evaluation of Token(k)"
    /// </summary>

    public class Token: IEvaluableExpression
    {
        public string token;
        public delegate object TokenEvaluatorFunction(object obj, string keyword);
        public virtual TokenEvaluatorFunction tokenEvaluator { get; set; }

        public Token(string token) 
        {
            this.token = token.Trim();
        }
        public Token(string token, TokenEvaluatorFunction tokenEvaluator) : this(token)
        {
            this.tokenEvaluator = tokenEvaluator;
        }
        public virtual object evaluate(object obj) 
        {
            if (this.tokenEvaluator == null)
            {
                throw new EvaluationException("No evaluation function exists for '" + token + "'");
            }
            return this.tokenEvaluator(obj, this.token);            
        }
        public override string ToString()
        {
            return this.token;
        }
    }
}
