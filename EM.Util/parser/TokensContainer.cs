using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser
{
    /// <summary>
    /// a helper class for the parser
    /// </summary>
    public class TokensContainer
    {
        EDictionary<string, string> tokens;
        EDictionary<string, IEvaluableExpression> expressions;   //keys may be of type token#statement_hash    (ex: token#-352765342)

        public TokensContainer()
        {
            tokens = new EDictionary<string, string>();
            expressions = new EDictionary<string, IEvaluableExpression>();
        }

        public bool ContainsKey(string k)
        {
            return (tokens.ContainsKey(k) || expressions.ContainsKey(k));
        }

        public bool isValueAnEvaluableExpression(string key)
        {
            return expressions.ContainsKey(key);
        }

        public IEvaluableExpression getExpression(string key)
        {
            return this.expressions[key];
        }

        public string getToken(string key)
        {
            return this.tokens[key];
        }
        public void AddToken(string key, string v)
        {
            this.tokens[key] = v;
        }
        public void AddEvaluableExpression(string k, IEvaluableExpression v)
        {
            this.expressions[k] = v;
        }
    }
}
