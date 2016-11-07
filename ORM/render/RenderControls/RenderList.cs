using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;
using System.Web.UI;
using EM.Collections;

namespace ORM.render.RenderControls
{
    public class RenderList : RenderGeneric
    {
        EList<object> keys;
        EList<object> values;

        public RenderList(GenericField fld, EList<object> keys, EList<object> values)  : base(fld) 
        {
            this.keys = keys;
            this.values = values;
        }

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", field.value != null ? field.value.ToString() : "NULL");
            }

            renderAttributes["name"] = field.name;
            string value = field.value != null ? field.value.ToString() : "";
            string res = "";

            string atr = getRenderAttr(renderAttributes);
            StringBuilder sres = new StringBuilder(string.Format("<select {0}>", atr));
            for (int i = 0; i < this.keys.Count; i++ )
            {
                object key = this.keys[i];
                object val = this.values[i];
                sres.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", val.ToString(), key.ToString(), field.value!= null && field.value.ToString() == val.ToString() ? " selected" : "");
            }
            sres.Append("</select>");
            
            res = this.renderError() + sres.ToString();
            return res;
        }

        public override Object renderReadOnly(RenderAttributes renderAttributes)
        {
            if (!renderAttributes.get("style", "").Contains("display: none;"))
            {
                string val = "&nbsp";
                if (field.value != null)
                {
                    if (this.OnValueFormat != null) { val = (string)this.OnValueFormat(field.value); }
                    else
                    {
                        int ix = this.values.IndexOf(field.value);
                        val = this.keys[ix].ToString();
                    }
                }
                return string.Format("<label>{0}</label><input type='hidden' name='{1}' value='{2}'>", val, field.name, field.value != null? field.value.ToString(): "");
            }
            return "";
        }

        

    }
}
