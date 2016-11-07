using System.Collections.Generic;
using System.Linq;
using EM.Collections;

namespace EM.DB.Index.Generic
{
    /// <summary>
    /// Use GenericLookup instead when approriate for better performance.
    /// This class is a mutable version of IGenericLookup as it exposes the internal dictionary used to implement it
    /// </summary>
    public class DictLookup<TK, TD> : ILookup<TK, TD>, IGenericLookup<TD>
    {
        public EDictionary<TK, EList<TD>> DataStore { get; set; }
        public ILookupFieldKey<TK, TD> KeyProvider { get; set; }
        protected bool ToUpper { get; set; }

        public DictLookup(IEnumerable<TD> tbl, ILookupFieldKey<TK, TD> keyProvider)
        {
            this.KeyProvider = keyProvider;
            this.DataStore = new EDictionary<TK, EList<TD>>();

            foreach (TD row in tbl)
            {
                TK ckey = this.KeyProvider.GetKey(row); 
                this.DataStore.setdefault(ckey, new EList<TD>()).Add(row);
            }
        }

        //public IEnumerable<TD> Select(TD valueObject)
        //{
        //    var key = this.KeyProvider.GetKey(valueObject);
        //    return this.DataStore.get(key, new EList<TD>());            
        //}
        public IEnumerable<TD> Select(IEnumerable<object> values)
        {
            var key = this.KeyProvider.GetKeyFromValues(values);
            return this.DataStore.get(key, new EList<TD>());            
        }

        public bool Has(IEnumerable<object> values)
        {
            var key = this.KeyProvider.GetKeyFromValues(values);
            return this.DataStore.ContainsKey(key);
        }

        public bool Contains(TK key)
        {
            return this.DataStore.ContainsKey(key);
        }

        public int Count
        {
            get { return this.DataStore.Count; }
        }

        public IEnumerable<TD> this[TK key]
        {
            get { return this.DataStore[key]; }
        }

        public IEnumerator<IGrouping<TK, TD>> GetEnumerator()
        {
            return this.DataStore.Keys.Select(key => new Grouping<TK, TD>(key, this.DataStore[key])).GetEnumerator();            
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();            
        }

    }

    public sealed class Grouping<TK, TD> : IGrouping<TK, TD>
    {
        private readonly TK _key;
        private readonly IEnumerable<TD> _elements;

        public Grouping(TK key, IEnumerable<TD> elements)
        {
            this._key = key;
            this._elements = elements;
        }

        public TK Key { get { return _key; } }

        public IEnumerator<TD> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}