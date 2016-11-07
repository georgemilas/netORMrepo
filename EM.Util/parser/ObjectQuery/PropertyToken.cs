using System;
using System.Collections.Generic;

namespace EM.parser.ObjectQuery
{
    public class PropertyToken : Token
    {
        public PropertyToken(string token): base(token) { }
        //private object evaled = null;
        public override object evaluate(object obj)
        {
            //if (evaled == null)
            //{
            var p = obj.GetType().GetProperty(token);
            if (p == null)
            {
                throw new EvaluationException(String.Format("Property {0} was not found", token));                    
            }
            var evaled = p.GetValue(obj, null);
            verify(evaled);
            //}
            return evaled;                
        }

        public void verify(object value)
        {
            var tp = value.GetType();
            List<Type> types = new List<Type>() { typeof(int), typeof(double), typeof(decimal), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset) };
            if (!types.Contains(tp))
            {
                throw new EvaluationException(String.Format("Property {0} is of type {1}. Supported types are int, double, decimal, string, DateTime and DateTimeOffset", token, tp));                
            }            
        }        
        
    }
}