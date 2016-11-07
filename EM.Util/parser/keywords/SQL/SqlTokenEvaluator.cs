using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.parser.keywords.SQL
{
    public class SQLTokenEvaluator
    {
        public enum OPERATOR_TYPE { EQUAL, LIKE , LIKE_AND_NULL_TEST };
        public enum FIELD_TYPE { STRING, NUMBER };
        protected EList<string> fields;
        protected OPERATOR_TYPE operatorType;
        protected FIELD_TYPE fieldType;

        public SQLTokenEvaluator(string field, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType):
            this(EList<string>.fromAray(new string[] { field }), operatorType, fieldType)
        {
            
        }
        public SQLTokenEvaluator(EList<string> fields, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType)
        {
            this.fields = fields;
            this.operatorType = operatorType;
            this.fieldType = fieldType;
        }

        //obj is always null, as we are not realy evaluating anything but transformin the kewords text into a SQL WHERE clause as string
        public object evaluator(object obj, string token)
        {
            EList<string> res = new EList<string>();
            foreach (string field in this.fields)
            {
                if (this.operatorType == OPERATOR_TYPE.LIKE || 
                    this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST
                   )
                {
                    if (this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST)
                    {
                        res.Add(string.Format("({0} IS NOT NULL AND {0} LIKE '%{1}%')", field, token));
                    }
                    else
                    {
                        res.Add(string.Format("{0} LIKE '%{1}%'", field, token));
                    }
                }
                else
                {
                    string str = "";
                    if (this.fieldType == FIELD_TYPE.STRING) { str = "'"; }
                    res.Add(string.Format("{0}={2}{1}{2}", field, token, str));
                }
            }
            return "(" + res.join(" OR ") + ")";
        }
    }

}
