using System;
using System.Collections.Generic;
using System.Linq;

namespace EM.Api.Core
{
    public class QueryStringValues : List<KeyValuePair<string, string>>
    {
        public QueryStringValues(IEnumerable<KeyValuePair<string, string>> data) : base(data) { }
        public string Get(string key)
        {
            var res = this.FirstOrDefault(kv => kv.Key.ToLower() == key.ToLower());
            if (res.Key != null)
            {
                return res.Value;
            }
            return null;
        }
        public bool GetBoolean(string key, bool defaultValue)
        {
            var res = this.Get(key);
            return res == null ? defaultValue : res.ToLower() == "true";            
        }
        public int GetInt(string key, int defaultValue = 0)
        {
            var res = this.Get(key);
            return res == null ? defaultValue : Convert.ToInt32(res);
        }
    }
}