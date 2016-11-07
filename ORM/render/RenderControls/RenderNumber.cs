using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    public class RenderNumber : RenderGeneric
    {
        public RenderNumber(FNumber fld) : base(fld) {}


        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            FNumber field = (FNumber)this.field;

            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", field.value != null ? field.value.ToString() : "NULL");
            }
            if (renderAttributes.fk_as != null && 
                renderAttributes.fk_as.tableThere != null &&
                !(this.field.table is TableRowStoredProcBased))  //No FK render support for Stored Proc Based ORM
            {
                //return this.renderFK(field, renderAttributes);
                RenderFK rfk = new RenderFK(field);
                return (string)rfk.render(renderAttributes);
            }
            else
            {
                renderAttributes["value"] = field.value != null ? field.value.ToString() : "";
                renderAttributes["name"] = field.name;
                renderAttributes["type"] = "text";
                string atr = getRenderAttr(renderAttributes);
                return this.renderError() + string.Format("<input {0}{1}>", atr, field.isIdentity ? " readonly style='border: none;'" : "");
            }
        }


        public override Object renderRequiredAsterix(RenderAttributes renderAttributes)
        {
            if (!this.field.isIdentity && this.field.isRequired)
            {
                return "<span class='fldRequired'>*</span>";
            }
            return "";
        }


    }
}
