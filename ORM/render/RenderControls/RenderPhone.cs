using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;
using System.Web.UI;
using EM.Collections;

namespace ORM.render.RenderControls
{
    /// <summary>
    /// render 6309800573 as 3 fields like:    | 630 | - | 980 | - | 0573 |
    /// </summary>
    public class RenderPhone : RenderText
    {
        
        public RenderPhone(FChar fld)  : base(fld) {}

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            FChar field = (FChar)this.field;

            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", field.value != null ? field.value.ToString() : "NULL");
            }
            renderAttributes["name"] = field.name;
            string res = "";
            
            if (field.length == 10)
            {
                renderAttributes["type"] = "text";
                string value = field.value != null ? field.value.ToString() : "";

                res = this.renderError();

                renderAttributes["name"] = field.name + "_1";
                renderAttributes["maxlength"] = "3"; 
                renderAttributes["size"] = "3";
                if (this.page != null)
                {
                    renderAttributes["value"] = this.page.Request.Params[field.name + "_1"];
                }
                else
                {
                    renderAttributes["value"] = StringUtil.slice(value, 0, 3);
                }
                string atr = getRenderAttr(renderAttributes);
                res += string.Format("<input {0}> - ", atr);

                renderAttributes["name"] = field.name + "_2";
                renderAttributes["maxlength"] = "3"; 
                renderAttributes["size"] = "3";
                if (this.page != null)
                {
                    renderAttributes["value"] = this.page.Request.Params[field.name + "_2"];
                }
                else
                {
                    renderAttributes["value"] = StringUtil.slice(value, 3, 6);
                }
                atr = getRenderAttr(renderAttributes);
                res += string.Format("<input {0}> - ", atr);

                renderAttributes["name"] = field.name + "_3";
                renderAttributes["maxlength"] = "4"; 
                renderAttributes["size"] = "4";
                if (this.page != null)
                {
                    renderAttributes["value"] = this.page.Request.Params[field.name + "_3"];
                }
                else
                {
                    renderAttributes["value"] = StringUtil.slice(value, 6);
                }
                atr = getRenderAttr(renderAttributes);
                res += string.Format("<input {0}>", atr);                
            }
            else 
            {
                renderAttributes["maxlength"] = renderAttributes.get("maxlength", field.length.ToString());
                if (field.length >= 35) renderAttributes["size"] = renderAttributes.get("size", "35");
                if (field.length < 10) renderAttributes["size"] = renderAttributes.get("size", (field.length + 1).ToString());
                renderAttributes["value"] = field.value != null ? field.value.ToString() : "";
                renderAttributes["type"] = "text";
                string atr = getRenderAttr(renderAttributes);
                res = this.renderError() + string.Format("<input {0}>", atr);
            }
            
            return res;
        }


        public override void readPageValue()
        {
            FChar field = (FChar)this.field;

            if (field.length == 10)
            {
                string val1 = this.page.Request.Params[this.field.name + "_1"];
                val1 = (val1 != null && val1.Trim() == "") ? null : val1;

                string val2 = this.page.Request.Params[this.field.name + "_2"];
                val2 = (val2 != null && val2.Trim() == "") ? null : val2;

                string val3 = this.page.Request.Params[this.field.name + "_3"];
                val3 = (val3 != null && val3.Trim() == "") ? null : val3;

                string val = val1 + val2 + val3;
                val = (val != null && val.Trim() == "") ? null : val;

                this.field.value = val;
            }
            else
            {
                base.readPageValue();
            }
            
        }




    }
}
