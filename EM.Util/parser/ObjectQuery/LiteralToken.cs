using System;

namespace EM.parser.ObjectQuery
{
    public class LiteralToken : Token
    {
        private PropertyToken prop;
        public LiteralToken(PropertyToken prop, string token): base(token)
        {
            this.prop = prop;
        }

        public override object evaluate(object obj)
        {
            var propValue = prop.evaluate(obj);
            Type tp = propValue.GetType();
            try
            {
                if (tp == typeof (int))
                {
                    return int.Parse(token);
                }
                if (tp == typeof (double))
                {
                    return double.Parse(token);
                }
                if (tp == typeof (decimal))
                {
                    return decimal.Parse(token);
                }
                if (tp == typeof (string))
                {
                    return token;
                }
                if (tp == typeof (bool))
                {
                    return bool.Parse(token);
                }
                if (tp == typeof (DateTime))
                {
                    return DateTime.Parse(token);
                }
                if (tp == typeof (DateTimeOffset))
                {
                    return DateTimeOffset.Parse(token);
                }
            }
            catch (FormatException)
            {
                throw new EvaluationException(string.Format("Input string was not in a correct format, expected {0} but found {1}", tp.Name, token));
            }
            throw new Exception("Impossible to get here in evaluate");
        }

        public bool compare(object obj, ObjectComparerOperator op)
        {
            var propValue = prop.evaluate(obj);
            var litValue = evaluate(obj);
            if (propValue is IComparable)
            {
                IComparable comparable = (IComparable) propValue;
                var res = comparable.CompareTo(litValue);

                if (op.op == "eq")
                {
                    return res == 0;
                }
                if (op.op == "ne")
                {
                    return res != 0;
                }
                if (op.op == "lt")
                {
                    return res < 0;
                }
                if (op.op == "gt")
                {
                    return res > 0;
                }
                if (op.op == "le")
                {
                    return res <= 0;
                }
                if (op.op == "ge")
                {
                    return res >= 0;
                }
                throw new Exception("Impossible to get here in compare");
            }
            throw new EvaluationException(String.Format("Values to compare are not IComparable {0}", token));                
        }
    }
}