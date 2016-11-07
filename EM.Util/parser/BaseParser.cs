using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser
{
    public abstract class BaseParser : IEvaluableExpression
    {
        public string keywordsExpression;

        protected TokensContainer tokensContainer;  //state helper
        protected EList<Token> _tokens;          //list of all tokens that are not expression trees themselfs
        protected delegate string TokenHandler(string token, int tokenHash);
        private IEvaluableExpression _expression;
        
        public BaseParser(string expressionToBeParsed)
        {
            this.keywordsExpression = expressionToBeParsed;
            this._expression = null;
        }

        /// <summary>
        /// tokenEvaluator if the current Keywords Expression matches the given text
        /// </summary>
        public virtual object evaluate(object obj)
        {
            return this.expression.evaluate(obj);
        }

        /// <summary>
        /// list of all tokens that are not expression trees themselfs
        /// </summary>
        protected EList<Token> tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }

        /// <summary>
        /// an evaluable expression tree 
        /// </summary>
        public IEvaluableExpression expression
        {
            get
            {
                if (this._expression == null)    //build an evaluable expression tree
                {
                    this.tokensContainer = new TokensContainer();
                    this.tokens = new EList<Token>();
                    string keys = this.keywordsExpression;

                    keys = this.prepareParsing(keys);
                    IEvaluableExpression exp = parse(keys);
                    this._expression = this.finalizeParsing(exp);
                }
                return _expression;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// prepare state machine and expressionToBeParsed into a clean parseable expressionToBeParsed
        /// </summary>
        /// <param name="expressionToBeParsed"></param>
        /// <returns></returns>
        public abstract string prepareParsing(string expressionToBeParsed);
        /// <summary>
        /// method that builds an evaluable expression tree from 
        /// </summary>
        public abstract IEvaluableExpression parse(string expressionToBeParsed);
        public abstract IEvaluableExpression finalizeParsing(IEvaluableExpression exp);




        /// <summary>
        /// Tokenize expresions between quotes 
        /// </summary>
        /// <returns></returns>
        protected string quotedStrings(string expressionToBeParsed)
        {
            return quotedStrings(expressionToBeParsed, '"', '\\');
        }
        protected string quotedStrings(string expressionToBeParsed, char quote, char quoteEscape)
        {
            string keys = expressionToBeParsed;

            //reduce expresions between quotes to some unique token that we can more easily work with
            int idx = keys.IndexOf(quote);
            while (idx >= 0)
            {
                int seekFrom = idx + 1;
                int idx2 = StringUtil.slice(keys, seekFrom).IndexOf(quote);

                while (idx2 >= 0 && keys[seekFrom + idx2 - 1] == quoteEscape && !(idx2 == keys.Length - 1))
                {
                    //escape for " is \" unless is the last char
                    seekFrom = seekFrom + idx2 + 1 + 1;
                    idx2 = StringUtil.slice(keys, seekFrom).IndexOf(quote);
                }
                if (idx2 >= 0)
                {
                    string token = StringUtil.slice(keys, idx, seekFrom + idx2 + 1);
                    int hs = token.GetHashCode();
                    tokensContainer.AddToken("token#" + hs.ToString(), StringUtil.slice(token, 1, -1).Replace(quoteEscape.ToString()+quote.ToString(), quote.ToString()));    //strip out enclosing quote symbol and un-escape quotes
                    keys = keys.Replace(token, "token#" + hs);
                    idx = keys.IndexOf(quote);
                }
                else
                {
                    break;
                }
            }
            
            return keys;
        }




        /// <summary>
        /// tokenize a statement and set in the TokensContainer 
        /// as  contaier[token#statement_hash] = IEvaluableExpression
        /// </summary>
        protected virtual string paranthesesTokenHandler(string token, int tokenHash)
        {
            //strip out '(' and ')' and recursively parse sub expression (sub parantheses)
            IEvaluableExpression tval = parse(StringUtil.slice(token, 1, -1));
            tokensContainer.AddEvaluableExpression("token#" + tokenHash.ToString(), tval);
            return "token#" + tokenHash.ToString();
        }

        /// <summary>
        /// tokenize a statement and set in the TokensContainer 
        /// as  contaier[token#statement_hash] = IEvaluableExpression
        /// </summary>
        protected virtual string parantheses(string kexp) 
        {
            return parantheses(kexp, paranthesesTokenHandler);
        }
        protected virtual string parantheses(string kexp, TokenHandler tokenHandler)
        {
            return parantheses(kexp, tokenHandler, '(', ')');
        }
        protected virtual string parantheses(string kexp, TokenHandler tokenHandler, char open, char close)
        {
            int openCnt = 0;
            EList<int> ixopen = new EList<int>();
            //first reduce expresions in between paranteses to an Expression instance:
            //uses something like a state machine (with a stack)
            for (int ix = 0; ix < kexp.Length; ix++)
            {
                if (kexp[ix] == open)
                {
                    openCnt += 1;  //add to stack
                    ixopen.Add(ix); //remember 
                }
                else if (kexp[ix] == close)
                {
                    openCnt -= 1;      //pop stack
                    if (openCnt == 0)  //we have the expresion
                    {
                        string token = StringUtil.slice(kexp, ixopen[0], ix + 1);
                        int hs = token.GetHashCode();
                        //parhanteses inside token must be handeled by this (may call parantheses 
                        //against token or whatever) and store the token value in the tokensContainer dictionary
                        string th = tokenHandler(token, hs);
                        kexp = kexp.Replace(token, th);
                        //restart to reduce more parantheses expresions (whether has more parantheses or no)
                        return parantheses(kexp, tokenHandler, open, close);
                    }
                    else
                    {
                        try { ixopen.pop(); }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ParsingException("parsing error - closing paranteses not matching open ones");
                        }
                    }
                }
            }
            if (openCnt != 0)
            {
                throw new ParsingException("parsing error - closing paranteses not matching open ones");
            }
            return kexp;
        }

        /// <summary>
        /// instantiate and return Token's, 
        ///  -this token does not yet have an evaluator function, one must be set at some point later 
        ///   maybe in finalizeParser method or something
        /// </summary>
        protected virtual IEvaluableExpression getToken(string r)
        {
            Token kw = null;

            if (tokensContainer.ContainsKey(r))
            {
                if (tokensContainer.isValueAnEvaluableExpression(r))
                {
                    return tokensContainer.getExpression(r);
                }
                kw = new Token(tokensContainer.getToken(r));
                this.tokens.Add(kw);
                return kw;
            }
            kw = new Token(r);
            this.tokens.Add(kw);
            return kw;
        }


    }

}
