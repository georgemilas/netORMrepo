using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using EM.Collections;

namespace EM.parser.keywords.TextSearch
{
    public class TextSearchSemantic: KeywordsSemantic
    {
        public TextSearchSemantic()
        {
            //this are short-circuit operators evaluators so maybe not everything will be evaluated
            this.AND = new SearchAND();
            this.OR = new SearchOR();
            this.NOT = new SearchNOT();
            this.tokenEvaluator = this.keywordFinder;
        }

        //obj is a string text 
        public object keywordFinder(object obj, string token)
        {
            string txt = (string) obj;
            //a regular expression keyword is between {} 
            if (token.StartsWith("{") && token.EndsWith("}"))
            {
                Regex r = new Regex(StringUtil.slice(token, 1, -1), RegexOptions.IgnoreCase);
                Match res = r.Match(txt);
                return res.Success;
            }
            //otherwise just tokenEvaluator for the text keyword
            return txt.IndexOf(token) >= 0;
        }

    }

}
