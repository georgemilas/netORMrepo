using System;
using System.Collections.Generic;
using EM.Collections;

namespace EM.DB.Index.Generic
{
    public class UniqueMultiFieldDictKeyProvider<T>: MultiFieldDictKeyProvider<T>
    {
        private Dictionary<MultiFieldDictKey, bool> backing = new Dictionary<MultiFieldDictKey, bool>();

        public UniqueMultiFieldDictKeyProvider(IEnumerable<string> fieldNames, bool toUpper, ILookupFieldGetter<T> fieldGetter) : base(fieldNames, toUpper, fieldGetter) { }
        public override MultiFieldDictKey GetKey(T row)
        {
            var key = new MultiFieldDictKey();
            foreach (var fn in this.FieldNames)
            {
                string fkey = this.FieldGetter.GetField(row, fn).ToString();
                if (this.ToUpper)
                {
                    fkey = fkey.ToUpper().Trim();
                }
                key.Add(fkey);
            }
            if (backing.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("Data is not unique for {0:True} in the table", EList<string>.fromEnumarable(this.FieldNames).ToString(true)));                
            }
            this.backing[key] = true;
            return key;
        }
    }
}