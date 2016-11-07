using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace EM.DB.Index.Generic
{
    public class MultiFieldDictKeyProvider<T>: ILookupFieldKey<MultiFieldDictKey, T>
    {
        public bool ToUpper { get; set; }
        public IEnumerable<string> FieldNames { get; set; }
        public ILookupFieldGetter<T> FieldGetter { get; set; }

        public MultiFieldDictKeyProvider(IEnumerable<string> fieldNames, bool toUpper, ILookupFieldGetter<T> fieldGetter)
        {
            this.ToUpper = toUpper;
            this.FieldNames = fieldNames;
            this.FieldGetter = fieldGetter;
        }
        public virtual MultiFieldDictKey GetKey(T data)
        {
            var key = new MultiFieldDictKey();
            foreach (var fn in this.FieldNames)
            {
                string fkey = FieldGetter.GetField(data, fn).ToString();
                if (this.ToUpper)
                {
                    fkey = fkey.ToUpper().Trim();
                }
                key.Add(fkey);
            }
            return key;
        }

        public virtual MultiFieldDictKey GetKeyFromValues(IEnumerable<object> values)
        {
            var valuesKey = new MultiFieldDictKey();
            foreach (var v in values)
            {
                if (this.ToUpper)
                {
                    valuesKey.Add(v.ToString().ToUpper().Trim());
                }
                else
                {
                    valuesKey.Add(v.ToString());
                }
            }
            return valuesKey;
        }        
    }
}
