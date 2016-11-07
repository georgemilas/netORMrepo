using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;

namespace ORM.render
{
    public class RenderAttributes : OrderedDictionary<string, string>
    {
        //represent a field as a combo box with values as in the related table
        public DBRelation fk_as;        
                                        

        //only render this fields (name, label) and ignore the rest of the fields in the table
        public OrderedDictionary<string, string> renderFields;
        public EList<string> skipRenderFields;

        //public delegate Object valueFormatter(Object value);
        //public valueFormatter valueFormat;

        /// <summary>
        /// {'key 1':'a value', 'key 2'  : 'other "value"'  ,<br></br>
        ///  'key 3':'george's value',
        /// 'key 4': 'other line'} ==> EDictionary&lt;string, string>
        /// -surround with {} and every key or value is in sigle quotes with anything inside (even other quotes)
        /// </summary>
        public static RenderAttributes fromStr(string litteral)
        {
            RenderAttributes r = new RenderAttributes();
            OrderedDictionary<string, string> src = OrderedDictionary<string, string>.fromStrStr(litteral);
            foreach (string k in src.Keys)
            {
                r.Add(k, src[k]);
            }           
            return r;            
        }

        public override string ToString()
        {
            if (this.Keys.Count <= 0) return "";

            EList<string> res = new EList<string>();
            foreach(string k in this.Keys) 
            {
                res.Add(string.Format("{0}=\"{1}\"", k, this[k]));
            }
            return res.join(" ");                   
        }

        public RenderAttributes copy() 
        {
            RenderAttributes res = new RenderAttributes();
            res.fk_as = this.fk_as;
            if (this.skipRenderFields != null)
            {
                res.skipRenderFields = this.skipRenderFields.copy();
            }
            if (this.renderFields != null)
            {
                res.renderFields = new OrderedDictionary<string, string>();
                foreach (string k in this.renderFields.Keys)
                {
                    res.renderFields.Add(k, this.renderFields[k]);
                }
            }
            foreach (string k in this.Keys)
            {
                res[k] = this[k];
            }
            return res;
        }



        /// <summary>
        /// Combine master and slave attributes into master 
        /// (if one attribute is in both master and slave it will keep the one from master)
        /// </summary>
        public static RenderAttributes combine(RenderAttributes master, RenderAttributes slave)
        {
            RenderAttributes _master;
            if (master == null) _master = new RenderAttributes();
            else _master = master.copy();

            RenderAttributes _slave = slave.copy();

            _master = (RenderAttributes)RenderAttributes._combine(_master, _slave);

            if (_slave.renderFields != null)
            {
                _master.renderFields = RenderAttributes._combine(_master.renderFields, _slave.renderFields);
            }
            if (_slave.skipRenderFields != null)
            {
                _master.skipRenderFields = RenderAttributes._combine(_master.skipRenderFields, _slave.skipRenderFields);
            }
            return _master;
        }


        /////////////////////////////////////////////////////////////////////////////////
        ///////////// HELPERS
        /////////////////////////////////////////////////////////////////////////////////
        protected static OrderedDictionary<string, string> _combine(OrderedDictionary<string, string> master, OrderedDictionary<string, string> slave)
        {
            if (slave.Keys.Count > 0)
            {
                if (master != null && master.Keys.Count > 0)
                {
                    foreach (string k in slave.Keys)
                    {
                        master[k] = master.get(k, slave[k]);
                    }
                }
                else
                {
                    master = slave;
                }
            }
            return master;
        }
        protected static EList<string> _combine(EList<string> master, EList<string> slave)
        {
            if (master == null) master = new EList<string>();
            if (slave.Count > 0)
            {
                foreach (string s in slave)
                {
                    if (!master.Contains(s))
                    {
                        master.Add(s);
                    }
                }
            }
            return master;
        }




    }
}
