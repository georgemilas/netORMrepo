using System;
using System.Collections.Generic;

namespace EM.DB.Index.Generic
{
    public class UniqueDictKeyProvider<T> : DictKeyProvider<T>
    {
        private Dictionary<string, bool> backing = new Dictionary<string, bool>();
        public UniqueDictKeyProvider(string fieldName, bool toUpper, ILookupFieldGetter<T> fieldGetter) : base(fieldName, toUpper, fieldGetter) { }
        public override string GetKey(T row)
        {
            string fkey = this.FieldGetter.GetField(row, this.FieldName).ToString();
            if (this.ToUpper)
            {
                fkey = fkey.ToUpper().Trim();
            }
            if (backing.ContainsKey(fkey))
            {
                throw new ArgumentException(String.Format("Data is not unique for {0} in the table", this.FieldName));
            }
            return fkey;
        }
    }
}