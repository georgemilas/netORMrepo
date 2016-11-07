using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EM.DB.Index.Generic
{
    public interface ILookupFieldKey<out TK, in T>
    {
        TK GetKeyFromValues(IEnumerable<object> values);
        TK GetKey(T row);
    }



    public abstract class StringLookupFieldGetter<T> : ILookupFieldKey<string, T>
    {
        public abstract string GetKey(T e);
        public string GetKeyFromValues(IEnumerable<object> values)
        {
            return GetStringLookupKey(values);
        }
        protected string GetStringLookupKey(IEnumerable<object> data)
        {
            StringBuilder s = new StringBuilder();
            var enumerable = data as object[] ?? data.ToArray();
            var first = enumerable[0];   //we know we always have 3 elements
            s.Append(first);
            foreach (var d in enumerable.Skip(1))
            {
                s.Append("|" + d);
            }
            return s.ToString();
        }
    }


}