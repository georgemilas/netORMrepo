using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace EM.Collections
{

    /// <summary>
    /// - an order dictionary on top of System.Collections.Generic.Dictionary[TK,TV]
    /// </summary>
    [Serializable]
    public class OrderedDictionary<TK, TV> : Dictionary<TK, TV>
    {

        public OrderedDictionary() : base() { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(IDictionary<TK, TV> dictionary) : base(dictionary) { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(IEqualityComparer<TK> comparer) : base(comparer) { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(int capacity) : base(capacity) { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(IDictionary<TK, TV> dictionary, IEqualityComparer<TK> comparer) : base(dictionary, comparer) { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(int capacity, IEqualityComparer<TK> comparer) : base(capacity, comparer) { this._keysOrdered = new ESet<TK>(); }
        public OrderedDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { this._keysOrdered = new ESet<TK>(); }

        public ESet<TK> _keysOrdered { get; set; }

        [Serializable]
        public class OrderedKeysCollection : IEnumerable<TK>
        {
            public OrderedDictionary<TK, TV> d { get; set; }
            public OrderedKeysCollection(OrderedDictionary<TK, TV> d) 
            {
                this.d = d; 
            }
           
            public IEnumerator GetEnumerator()
            {
                foreach (TK k in d._keysOrdered)
                {
                    yield return k;
                }
            }
            IEnumerator<TK> IEnumerable<TK>.GetEnumerator()
            {
                foreach (TK k in d._keysOrdered)
                {
                    yield return k;
                }
            }


            public int Count
            {
                get { return this.d._keysOrdered.Count; }
            }

            
        }

        [Serializable]
        public class OrderedValuesCollection : IEnumerable<TV>
        {
            public OrderedDictionary<TK, TV> d { get; set; }
            public OrderedValuesCollection(OrderedDictionary<TK, TV> d)
            {
                this.d = d;
            }

            public IEnumerator GetEnumerator()
            {
                foreach (TK k in d._keysOrdered)
                {
                    yield return this.d[k];
                }
            }
            IEnumerator<TV> IEnumerable<TV>.GetEnumerator()
            {
                foreach (TK k in d._keysOrdered)
                {
                    yield return this.d[k];
                }
            }

            public int Count
            {
                get { return this.d._keysOrdered.Count; }
            }

        }

        

        public new void Add(TK k, TV v)
        {
            base.Add(k, v);
            this._keysOrdered.Add(k);
        }
        public new bool Remove(TK k)
        {
            bool res = base.Remove(k);
            if (res) 
            {
                this._keysOrdered.Remove(k);
            }
            return res;
        }
        public new void Clear()
        {
            base.Clear();
            this._keysOrdered.Clear();
        }
        public new OrderedKeysCollection Keys     //TODO: we could ovveride base one but must return a KeyCollection instance and don't know how to build one
        {
            get 
            {
                return new OrderedKeysCollection(this);
            }
        }
        public ESet<TK> keysList
        {
            get 
            {
                return this._keysOrdered.copy();
            }
        }
        public new OrderedValuesCollection Values   //TODO: we could ovveride base one but must return ItemsCollection
        {
            get
            {
                return new OrderedValuesCollection(this);
            }
        }
        public new TV this[TK k] 
        {
            get 
            {
                return base[k];
            }
            set
            {
                base[k] = value;
                this._keysOrdered.Add(k);
            }
        }

        
        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////  NOW SAME AS EDictionary
        /////////////////////////////////////////////////////////////////////////////////////////////
        
        private static string litteralFixup(string litteral)
        {
            litteral = litteral.Trim();
            if (!(litteral.StartsWith("{") && litteral.EndsWith("}")))
            {
                throw new NotSupportedException("A dictionary litteral must start and end with { respective }");
            }
            litteral = litteral.Substring(1, litteral.Length - 2);   //strip out {}
            litteral = litteral.Trim();
            if (!litteral.EndsWith(",")) litteral = litteral + ",";  //every key/value pair must end in comma 
            return litteral;

        }
        /// <summary>
        /// {'key 1':1, 'george's key':5, 
        ///  'key 3':2123,
        ///  'key 4 other line': 255} ==> EDictionary&lt;string, int>
        ///  -surround with {} and every key is in sigle quotes with anything inside (even other quotes)
        /// </summary>
        public static OrderedDictionary<string, int> fromStrInt(string litteral)
        {
            litteral = OrderedDictionary<string, int>.litteralFixup(litteral);

            OrderedDictionary<string, int> di = new OrderedDictionary<string, int>();
            Regex dStrInt = new Regex("\\s*(?<key>'.*?')\\s*:\\s*(?<value>\\d+)\\s*,");

            MatchCollection matches = dStrInt.Matches(litteral);
            for (int i = 0; i != matches.Count; ++i)
            {
                di.Add(matches[i].Groups["key"].ToString().Substring(1, matches[i].Groups["key"].ToString().Length - 2), int.Parse(matches[i].Groups["value"].ToString()));
            }

            return di;
        }

        /// <summary>
        /// {'key 1':'a value', 'key 2'  : 'other "value"'  ,<br></br>
        ///  'key 3':'george's value',
        /// 'key 4': 'other line'} ==> EDictionary&lt;string, string>
        /// -surround with {} and every key or value is in sigle quotes with anything inside (even other quotes)
        /// </summary>
        public static OrderedDictionary<string, string> fromStrStr(string litteral)
        {
            litteral = OrderedDictionary<string, string>.litteralFixup(litteral);

            OrderedDictionary<string, string> ds = new OrderedDictionary<string, string>();
            Regex dStrStr = new Regex("\\s*(?<key>'.*?')\\s*:\\s*(?<value>'.*?')\\s*,");

            MatchCollection matches = dStrStr.Matches(litteral);
            for (int i = 0; i != matches.Count; ++i)
            {
                ds.Add(matches[i].Groups["key"].ToString().Substring(1, matches[i].Groups["key"].ToString().Length - 2), matches[i].Groups["value"].ToString().Substring(1, matches[i].Groups["value"].ToString().Length - 2));
            }

            return ds;
        }

        /// <summary>
        /// - get the n-th key of the dictionary (this is an ordered collection)
        /// - 0 based index
        /// </summary>
        public TK keyByIndex(int idx)
        {
            int ct = 0;
            foreach (TK k in this.Keys)
            {
                if (ct == idx)
                {
                    return k;
                }
                else
                {
                    ct++;
                }
            }
            throw new IndexOutOfRangeException();
        }

        public virtual NameValueCollection ToNameValueCollection()
        {
            NameValueCollection c = new NameValueCollection();
            foreach (TK k in this.Keys)
            {
                c.Add(k.ToString(), this[k].ToString());
            }
            return c;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{" + StringUtil.CRLF);
            foreach (TK k in this.Keys)
            {
                sb.AppendFormat("{0} = {1}" + StringUtil.CRLF, k.ToString(), this[k] != null ? this[k].ToString() : "null");
            }
            sb.Append("}" + StringUtil.CRLF);
            return sb.ToString();
        }

        /// <summary>
        /// get the value of "key" and if key/value does not exist yet then return "defaultValue"
        /// </summary>
        public virtual TV get(TK key, TV defaultValue)
        {
            if (this.ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// get the value of "key". - If no value exist yet then first set "value" and then return it.
        ///                         - If a value exist then just return existing one
        /// </summary>
        public virtual TV setdefault(TK key, TV value)
        {   //like in a python dictionary
            if (this.ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                this[key] = value;
                return this[key];
            }
        }

    }
}
