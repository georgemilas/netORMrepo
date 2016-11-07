using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;
using System.Linq;

namespace EM.DB.Index.Generic
{
    public class MultiFieldDictKey
    {
        private List<string> Items { get; set; }

        public MultiFieldDictKey()
        {
            this.Items = new List<string>();            
        }
        public MultiFieldDictKey(IEnumerable<string> items): this()
        {
            this.Items.AddRange(items);            
        }
        public void Add(string field)
        {
            this.Items.Add(field);
        }

        private int? _hashcode;  //only compute hashcode once, the items it represents is imutable
        public override int GetHashCode()
        {
            if (_hashcode == null)
            {
                int hc = 0;
                foreach (var i in this.Items)
                {
                    hc = hc | i.GetHashCode();
                }
                _hashcode = hc;
            }
            return _hashcode.Value;
        }

        public override bool Equals(object obj)
        {
            var other = (MultiFieldDictKey)obj;
            if (this.Items.Count != other.Items.Count) return false;
            bool ok = true;
            for (int i = 0; i < this.Items.Count; i++)
            {
                ok = ok && (this.Items[i] == other.Items[i]);
            }
            return ok;
        }

        private string _representation = null;
        public override string ToString()
        {
            if (_representation == null)
            {
                _representation = this.Items.ToString();
            }
            return _representation;
        }
    }
}
