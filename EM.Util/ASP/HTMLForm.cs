using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using EM.Collections;

namespace EM.ASP
{
    public class HTMLForm
    {
        public Page page;

        public HTMLForm(Page page)
        {
            this.page = page;
        } 


        public string options(DataTable tb) { return options(tb, null, null); }
        public string options(DataTable tb, string name, string firstOption)
        {
            if (firstOption == null) { firstOption = ""; }
            string options = "";
            string selval = "";
            if (name != null && name.Trim() != "")
            {
                selval = this.page.Request.Params[name];
            }

            foreach (DataRow r in tb.Rows)
            {
                options += string.Format("<option value=\"{0}\"{2}>{1}</option>", r[0].ToString(), r[1].ToString(), selval == r[0].ToString() ? " selected" : "");
            }
            return firstOption + options;
        }
        
        /// <summary>
        ///  - atr must contain a name attribute
        ///  - selected value is take from page.request.params[atr_name]
        ///  - first option you either add in DataTable or there is none
        /// </summary>
        public string select(OrderedDictionary<string, string> atr, DataTable tb) { return select(atr, tb, null); }
        /// <summary>
        ///  - atr must contain a name attribute
        ///  - selected value is take from page.request.params[atr_name]
        /// </summary>
        public string select(OrderedDictionary<string, string> atr, DataTable tb, string firstOption)
        {
            EList<string> anames = new EList<string>();
            foreach (string k in atr.Keys)
            {
                anames.Add(k + "=\"" + atr[k] + "\"");
            }
            
            string name = atr.get("name", null);
            string options = this.options(tb, name, firstOption) ;
            return string.Format("<select {0}>{1}</select>", anames.join(" "), options);
        }


        /// <summary>
        /// the selected value you give yourself
        /// </summary>
        public string select(string name, DataTable tb, object selected, string first, string attributes)
        {
            attributes = attributes != null ? " " + attributes : "";
            StringBuilder res = new StringBuilder("<select name=\"" + name + "\" id=\"" + name + "\"" + attributes + ">");
            if (first != null) res.Append(first);
            foreach (DataRow r in tb.Rows)
            {
                res.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", r[0].ToString(), r[1].ToString(), (selected != null && selected.ToString() == r[0].ToString()) ? " selected" : "");
            }
            res.Append("</select>");
            return res.ToString();

        }
        /// <summary>
        /// the selected value you give yourself
        /// </summary>
        public string select(string name, EList<object> lst, object selected, string first, string attributes)
        {
            attributes = attributes != null ? " " + attributes : "";
            StringBuilder res = new StringBuilder("<select name=\"" + name + "\" id=\"" + name + "\"" + attributes + ">");
            if (first != null) res.Append(first);
            lst = (EList<object>)lst[0];
            foreach (object o in lst)
            {
                EList<object> r = (EList<object>)o;
                res.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", r[0].ToString(), r[1].ToString(), (selected != null && selected.ToString() == r[0].ToString()) ? " selected" : "");
            }
            res.Append("</select>");
            return res.ToString();
        }




    }
}
