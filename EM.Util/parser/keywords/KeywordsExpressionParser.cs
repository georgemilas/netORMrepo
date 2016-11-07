using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using EM.Collections;

namespace EM.parser.keywords
{

    /// <summary>
    ///   - space(s) is an OR, & is an AND, ! is a NOT
    ///   - "k1 k2" same as "k1 or k2" 
    ///   - "k1 k2 & !k3" same as "k1 or (k2 and not k3)"
    ///   - 'k1 "some phrase"' same as 'k1 or "some phrase"' 
    ///   - NOTE quotes " inside "some phrase" must be escaped with \ so "some\" phrase" and 
    ///     if \" is nedded then "some \\"phrase" because (\") will become (") after parsig so no need to excape excaping
    ///                          "some \\\"phrase"  -> some \\"phrase
    ///                          "some \\\\"phrase"  -> some \\\"phrase
    ///   - k1 (k2 & {k3}) == k1 or (k2 and {k3}) where k3 may be a regular expresion
    ///   - precedence table in descending order: {exp} "exp" ! and or
    /// </summary>
    public class KeywordsExpressionParser: BaseParser
    {
        private IKeywordsSemantic _semantic;
        public IKeywordsSemantic semantic
        {
            get { return _semantic; }
            set { _semantic = value; }
        }
        
        public KeywordsExpressionParser(string keywordsExpression): this(keywordsExpression, new TextSearch.TextSearchSemantic()) {}
        public KeywordsExpressionParser(string keywordsExpression, IKeywordsSemantic semantic)
            : base(keywordsExpression)
        {
            this.semantic = semantic;
        }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////
        private string curlyBracesTokenHandler(string token, int tokenHash)
        {
            //just store the token as is (curlyBraces inside are part of the regular expresion)
            //when matching, it will check for token[0]=={ & token[-1]==} and if yes it will use 
            //regexp matching if available or literal matching of the whole thing including {} otherwise
            this.tokensContainer.AddToken("token#" + tokenHash.ToString(), token);
            return "token#" + tokenHash.ToString();
        }

        public override string prepareParsing(string keys)
        {
            //1. handle Rexexp first
            //expresions inside {} are regular expresions, tokenize them
            keys = this.parantheses(keys, this.curlyBracesTokenHandler, '{', '}');

            //2. handle "exact match strings" second
            //reduce expresions between quotes to some unique token that we can more easily work with
            keys = quotedStrings(keys);
            
            //3. align tokensContainer and expressions one next to the other 
            //since whitespace has a meaning in this language
            Regex r = new Regex(@"\s+");
            keys = r.Replace(keys, " ");    //reduce multiple spaces to 1 space

            return keys;
        }

        public override IEvaluableExpression finalizeParsing(IEvaluableExpression exp)
        {
            if (this.semantic != null && this.tokens != null)
            {
                foreach (Token k in this.tokens)
                {
                    k.tokenEvaluator = this.semantic.tokenEvaluator;
                }
            }
            return exp;
        }

        public override IEvaluableExpression parse(string kexp)
        {
            kexp = kexp.Trim();
            kexp = parantheses(kexp, paranthesesTokenHandler);
            //all expresions between paranteses are toketized and so are Regex and literals
            //so reduce "and, or, not" with the rest of tokens
            kexp = kexp.Replace(" not ", " |not|");
            kexp = kexp.Replace(" ! ", " |not|");
            kexp = kexp.Replace(" !", " |not|");
            kexp = kexp.Replace(" and ", "|and|");
            kexp = kexp.Replace(" & ", "|and|");
            kexp = kexp.ToLower().Replace(" or ", "|or|");
            kexp = kexp.Replace(" ", "|or|");

            //OR has lower precedence then AND, 
            //so split OR's, split AND's then do AND's and then do OR's
            EList<string> orList = StringUtil.split(kexp, "|or|");   
            EList<IEvaluableExpression> ors = new EList<IEvaluableExpression>();
            IEvaluableExpression exp = null;
            if (orList.Count > 1)
            {
                foreach (string k in orList)
                {
                    //will reduce "not" before reducing "and parts":
                    ors.Add(getAndExpression(k));
                }
                exp = new ExpressionTree(this.semantic.OR, ors);
            }
            else
            {
                exp = getAndExpression(orList[0]);
            }

            return exp;
        }    

        protected IEvaluableExpression getAndExpression(string k)
        {
            EList<string> andList= StringUtil.split(k, "|and|"); 
            if ( andList.Count > 1 )
            {
                EList<IEvaluableExpression> ands = new EList<IEvaluableExpression>();
                foreach (string a in andList)
                {
                    if (a.Contains("|not|"))
                    {
                        string ka = a.Replace("|not|", "");
                        ands.Add(new ExpressionTree(this.semantic.NOT, getToken(ka)));
                    }
                    else
                    {
                        ands.Add(getToken(a));
                    }
                }
                return new ExpressionTree(this.semantic.AND, ands);
            }
            else 
            {

                if (tokensContainer.ContainsKey(k))
                {
                    if (tokensContainer.isValueAnEvaluableExpression(k))
                    {
                        return tokensContainer.getExpression(k);
                    }
                    else
                    {
                        string token = tokensContainer.getToken(k);
                        if (token.Contains("|not|"))
                        {
                            Token kw = new Token(token.Replace("|not|", ""));
                            this.tokens.Add(kw);
                            return new ExpressionTree(this.semantic.NOT, kw);
                        }
                        else
                        {
                            Token kw = new Token(token);
                            this.tokens.Add(kw);
                            return kw;
                        }
                    }
                }
                else
                {
                    Token kw = new Token(k);
                    this.tokens.Add(kw);
                    return kw;
                }             
                
            }
        }        

        
    }


    

}
