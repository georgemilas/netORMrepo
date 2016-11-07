using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace EM.Collections
{   
    [Serializable]
    public class EDictionary<TK, TV> : Dictionary<TK, TV>
    {
        public EDictionary():base(){}
        public EDictionary(IDictionary<TK, TV> dictionary) : base(dictionary) { }
        public EDictionary(IEqualityComparer<TK> comparer) : base(comparer) { }
        public EDictionary(int capacity) : base(capacity) { }
        public EDictionary(IDictionary<TK, TV> dictionary, IEqualityComparer<TK> comparer) : base(dictionary, comparer) { }
        public EDictionary(int capacity, IEqualityComparer<TK> comparer) : base(capacity, comparer) { }
        public EDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }


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
        public static EDictionary<string, int> fromStrInt(string litteral)
        {
            litteral = EDictionary<string, int>.litteralFixup(litteral);

            EDictionary<string, int> di = new EDictionary<string, int>();
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
        ///  'key 4': 'other line'} ==> EDictionary&lt;string, string>
        /// -surround with {} and every key or value is in sigle quotes with anything inside (even other quotes)
        /// </summary>
        public static EDictionary<string, string> fromStrStr(string litteral)
        {
            litteral = EDictionary<string, string>.litteralFixup(litteral);

            EDictionary<string, string> ds = new EDictionary<string, string>();
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
            return ToString((v) => v.ToString());             
        }
        public string ToString(Func<TV, string> valueToStringFunc)
        {
            return ToString((k) => k.ToString(), valueToStringFunc);             
        }
        public string ToString(Func<TK, string> keyToStringFunc, Func<TV, string> valueToStringFunc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{" + StringUtil.CRLF);
            foreach (TK k in this.Keys)
            {
                sb.AppendFormat("{0} = {1}" + StringUtil.CRLF, keyToStringFunc(k), this[k] != null ? valueToStringFunc(this[k]) : "null");
            }
            sb.Append("}" + StringUtil.CRLF);
            return sb.ToString();
        }

        public EList<TK> sortedKeys
        {
            get
            {
                EList<TK> res = new EList<TK>();
                foreach (TK k in this.Keys)
                {
                    res.Add(k);
                }
                res.Sort();
                return res;
            }
        }

        /// <summary>
        /// ret a dictionary where keys are sorted
        /// </summary>
        public EDictionary<TK, TV> sorted()
        {
            EDictionary<TK, TV> res = new EDictionary<TK, TV>();
            foreach(TK k in this.sortedKeys)
            {
                res[k] = this[k];
            }
            return res;
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

    //public static class DictionaryExtensions
    //{
    //    public static TK keyByIndex<TK, TV>(this Dictionary<TK, TV> dct, int idx)
    //    {
    //        int ct = 0;
    //        foreach (TK k in dct.Keys)
    //        {
    //            if (ct == idx)
    //            {
    //                return k;
    //            }
    //            else
    //            {
    //                ct++;
    //            }
    //        }
    //        throw new IndexOutOfRangeException();
    //    }

    //    public static NameValueCollection ToNameValueCollection<TK, TV>(this Dictionary<TK, TV> dct)
    //    {
    //        NameValueCollection c = new NameValueCollection();
    //        foreach (TK k in dct.Keys)
    //        {
    //            c.Add(k.ToString(), dct[k].ToString());
    //        }
    //        return c;
    //    }


    //    /// <summary>
    //    /// get the value of "key" and if key/value does not exist yet then return "defaultValue"
    //    /// </summary>
    //    public static TV get<TK, TV>(this Dictionary<TK, TV> dct, TK key, TV defaultValue)
    //    {
    //        return dct.ContainsKey(key) ? dct[key] : defaultValue;
    //    }

    //    /// <summary>
    //    /// get the value of "key". - If no value exist yet then first set "value" and then return it.
    //    ///                         - If a value exist then just return existing one
    //    /// </summary>
    //    public static TV setdefault(this Dictionary<TK, TV> dct, TK key, TV value)
    //    {   
    //        //like in a python dictionary
    //        if (dct.ContainsKey(key))
    //        {
    //            return dct[key];
    //        }
    //        else
    //        {
    //            dct[key] = value;
    //            return dct[key];
    //        }
    //    }


    //}

}
