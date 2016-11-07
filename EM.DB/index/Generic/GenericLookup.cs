using System;
using System.Collections.Generic;
using System.Linq;

namespace EM.DB.Index.Generic
{
    public interface IGenericLookup<out TD>
    {
        IEnumerable<TD> Select(IEnumerable<object> values);
        bool Has(IEnumerable<object> values);
    }

    public class GenericLookup<TK, TD>: ILookup<TK, TD>, IGenericLookup<TD>
    {
        protected ILookup<TK, TD> DataStore { get; set; }
        public ILookupFieldKey<TK, TD> KeyProvider { get; set; }

        public GenericLookup(IEnumerable<TD> tbl, ILookupFieldKey<TK, TD> keyProvider)
        {
            this.KeyProvider = keyProvider;
            this.DataStore = tbl.ToLookup(keyProvider.GetKey);
        }

        //public IEnumerable<TD> Select(TD valueObject)
        //{
        //    var key = this.KeyProvider.GetKey(valueObject);
        //    return this.DataStore[key];
        //}
        public IEnumerable<TD> Select(IEnumerable<object> values)
        {
            var key = this.KeyProvider.GetKeyFromValues(values);
            return this.DataStore[key];            
        }

        public bool Has(IEnumerable<object> values)
        {
            var key = this.KeyProvider.GetKeyFromValues(values);
            return this.DataStore.Contains(key);
        }

        public bool Contains(TK key)
        {
            return this.DataStore.Contains(key);
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
            return this.DataStore.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.DataStore.GetEnumerator();
        }
    }
}
