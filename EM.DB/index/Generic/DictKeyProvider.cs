using System.Collections.Generic;
using System.Linq;

namespace EM.DB.Index.Generic
{

    //- we use strings for keys so that we can match numbers for example we may have a double and a decimal, 
    //  both the same number but they would not match otherwise 
    //- we use toUpper to allow strings in diferent casing to match
    public class DictKeyProvider<T> : ILookupFieldKey<string, T>
    {
        public bool ToUpper { get; set; }
        public string FieldName { get; set; }
        public ILookupFieldGetter<T> FieldGetter { get; set; }

        public DictKeyProvider(string fieldName, bool toUpper, ILookupFieldGetter<T> fieldGetter) 
        {
            this.ToUpper = toUpper;
            this.FieldName = fieldName;
            this.FieldGetter = fieldGetter;
        }
        public virtual string GetKey(T row)
        {
            string fkey = this.FieldGetter.GetField(row, this.FieldName).ToString();
            if (this.ToUpper)
            {
                fkey = fkey.ToUpper().Trim();
            }
            return fkey;
        }

        public virtual string GetKeyFromValues(IEnumerable<object> values)
        {
            var v = values.First().ToString();
            if (this.ToUpper)
            {
                v = v.ToUpper().Trim();
            }                        
            return v;
        }
    }
}