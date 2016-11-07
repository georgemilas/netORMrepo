using System;
using System.Collections.Generic;
using System.Text;
using ORM.exceptions;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    public class RenderBool : RenderGeneric
    {
        public RenderBool(FBoolean fld): base(fld) {}


        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            renderAttributes["value"] = "1";
            renderAttributes["name"] = field.name;
            renderAttributes["type"] = "checkbox";
            string atr = getRenderAttr(renderAttributes);
            bool chk = false;

            if (field.value != null)
            {
                if (field.value is bool) chk = (bool)field.value;
                else
                {
                    if (field.value is string)
                    {
                        chk = (string)field.value == "1" ? true : false;        //coming as a post back and field was not validated (probably should not happen)
                    }
                    else
                    {   //should not ever get here unless manualy setting something bogus in code in field value and not validating
                        ValidationException ve = new ValidationException("This field must be a boolean or string(0/1) " + field.table.dbObjectName.table + "." + field.name);
                        ve.fieldName = field.name;
                        //throw new ValidationException("This field must be a boolean or string(0/1) " + field.table.dbObjectName.table + "." + field.name);
                        throw ve;
                    }
                }
            }
            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", chk);
            }
            return this.renderError() + string.Format("<input {0}{1}>", atr, chk ? " checked" : "");
        }

        public override Object renderReadOnly(RenderAttributes renderAttributes)
        {
            if (!renderAttributes.get("style", "").Contains("display: none;"))
            {
                string val = "NULL";
                if (field.value != null)
                {
                    if (this.OnValueFormat != null) { val = (string)this.OnValueFormat(field.value); }
                    else val = field.value.ToString();
                }
                return string.Format("<label>{0}</label>", val);
            }
            return "";
            //return string.Format("<input {0}{1}>", stm, chk ? " checked" : "");
        }
        
       
    }
}
