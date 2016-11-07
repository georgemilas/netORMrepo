using EM.Collections;
using EM.parser.keywords;
using System.Text.RegularExpressions;

namespace EM.parser.ObjectQuery
{
    public class ObjectQueryExpressionParser : KeywordsExpressionParser
    {
        public ObjectQueryExpressionParser(string apiQueryExpression, ObjectQuerySemantic semantic): base(apiQueryExpression, semantic) { }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////

        public string ParseReplace(string kexp, string what, string replacement)
        {
            Regex rex = new Regex(what, RegexOptions.IgnoreCase);
            return rex.Replace(kexp, replacement);
        }

        public override IEvaluableExpression parse(string kexp)
        {
            kexp = kexp.Trim();
            kexp = parantheses(kexp, paranthesesTokenHandler);
            //all expresions between paranteses are toketized and so are Regex and literals
            //so reduce "and, or, not" with the rest of tokens
            
            kexp = ParseReplace(kexp, " le ", "|le|");
            kexp = ParseReplace(kexp, " *<= *", "|le|");    //must handle '<=' before any of '=' or '<'  

            kexp = ParseReplace(kexp, " ge ", "|ge|");
            kexp = ParseReplace(kexp, " *>= *", "|ge|");

            kexp = ParseReplace(kexp, " ne ", "|ne|");
            kexp = ParseReplace(kexp, " *!= *", "|ne|");

            kexp = ParseReplace(kexp, " eq ", "|eq|");
            kexp = ParseReplace(kexp, " *= *", "|eq|");
            
            kexp = ParseReplace(kexp, " lt ", "|lt|");
            kexp = ParseReplace(kexp, " *< *", "|lt|");
            
            kexp = ParseReplace(kexp, " gt ", "|gt|");
            kexp = ParseReplace(kexp, " *> *", "|gt|");
            
            

            kexp = ParseReplace(kexp, " not ", " |not|");
            kexp = ParseReplace(kexp, " and ", "|and|");
            kexp = ParseReplace(kexp, " or ", "|or|");
            //kexp = kexp.Replace(" ", "|or|");   // we are not supporting space " " as an OR for the API language

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
            EList<string> andList = StringUtil.split(k, "|and|");
            if (andList.Count > 1)
            {
                EList<IEvaluableExpression> ands = new EList<IEvaluableExpression>();
                foreach (string a in andList)
                {
                    if (a.Contains("|not|"))
                    {
                        string ka = a.Replace("|not|", "");
                        ands.Add(new ExpressionTree(this.semantic.NOT, getComparerOperatorExpression(ka)));
                    }
                    else
                    {
                        ands.Add(getComparerOperatorExpression(a));
                    }
                }
                return new ExpressionTree(this.semantic.AND, ands);
            }
            else
            {
                if (tokensContainer.ContainsKey(k))
                {
                    return getNotExpression(k);  //not takes precedence over comparer operators so "not a eq b" is the same as "(not a) eq b"
                }
                else
                {
                    if (k.Contains("|not|"))
                    {
                        string ka = k.Replace("|not|", "");
                        return new ExpressionTree(this.semantic.NOT, getComparerOperatorExpression(ka));
                    }
                    else
                    {
                        return getComparerOperatorExpression(k);
                    }                    
                }

            }
        }

        //not takes precedence over comparer operators so "not a eq b" is the same as "(not a) eq b"
        private IEvaluableExpression getNotExpression(string k)
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
                    string ka = token.Replace("|not|", "");
                    return new ExpressionTree(this.semantic.NOT, getComparerOperatorExpression(ka));                    
                }
                else
                {
                    Token kw = new Token(token);
                    this.tokens.Add(kw);
                    return kw;
                }
            }
        }

        /// <summary>
        /// opToken -> |eq|, |ne|, etc.
        /// </summary>
        protected IEvaluableExpression getComparerOperatorExpression(string r)
        {
            ObjectQuerySemantic s = (ObjectQuerySemantic)this.semantic;
            var cmp = getOperator(r, "|eq|", s.EQ);
            if (cmp == null) cmp = getOperator(r, "|ne|", s.NE);
            if (cmp == null) cmp = getOperator(r, "|lt|", s.LT);
            if (cmp == null) cmp = getOperator(r, "|gt|", s.GT);
            if (cmp == null) cmp = getOperator(r, "|le|", s.LE);
            if (cmp == null) cmp = getOperator(r, "|ge|", s.GE);
            if (cmp == null)
            {
                return getToken(r);    
            }
            return cmp;
        }

        /// <summary>
        /// opToken -> |eq|, |ne|, etc.
        /// </summary>
        protected IEvaluableExpression getOperator(string r, string opToken, IOperator op)
        {
            //We have Support for Property to Literal comparison 
            //There is no support for Property to Property comparison 
            //There is no support for Literal to Literal comparison 

            EList<string> opList = StringUtil.split(r, opToken);
            if (opList.Count > 1)
            {
                var propToken = new PropertyToken(opList[0]);          //left side -> property must be on the left  
                this.tokens.Add(propToken);

                string literal = tokensContainer.ContainsKey(opList[1]) ? tokensContainer.getToken(opList[1]) : opList[1];
                var litToken = new LiteralToken(propToken, literal);  //right side -> literal must be on the right                             
                this.tokens.Add(litToken);

                return new OperatorExpression(op, propToken, litToken);                                                
            }
            return null;
        }

    }
}