using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace EM.DB.Index
{
    public class MultiFileldDictKey
    {
        private EList<string> items { get; set; }

        public MultiFileldDictKey(EList<string> items)
        {
            this.items = items;
        }
        public override int GetHashCode()
        {
            int hc = 0;
            foreach (var i in items)
            {
                hc = hc | i.GetHashCode();
            }
            return hc;
        }

        public IEnumerable<string> getItems()
        {
            return items;
        }

        public override bool Equals(object obj)
		{
		    MultiFileldDictKey other = (MultiFileldDictKey)obj;
		    if (this.items.Count != other.items.Count) return false;
		    bool ok = true;
		    for(int i=0; i < this.items.Count; i++)
		    {
                ok = ok && (items[i] == other.items[i]);
		    }
		    return ok;
		}

        public override string ToString()
        {
            return items.ToString();
        }
    }
}
