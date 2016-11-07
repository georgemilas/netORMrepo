using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    public class RenderText : RenderGeneric
    {
        public RenderText(FChar fld)  : base(fld) {}

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            FChar field = (FChar)this.field;

            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", field.value != null ? field.value.ToString() : "NULL");
            }
            renderAttributes["name"] = field.name;
            string res = "";
            if (renderAttributes.fk_as != null && 
                renderAttributes.fk_as.tableThere != null && 
                !(this.field.table is TableRowStoredProcBased))  //No FK render support for Stored Proc Based ORM
            {
                //res = this.renderFK(field, renderAttributes);
                RenderFK rfk = new RenderFK(field);
                res = (string)rfk.render(renderAttributes);
            }
            else
            {
                if (field.length < 150 && field.length > 0)
                {
                    renderAttributes["maxlength"] = renderAttributes.get("maxlength", field.length.ToString());
                    if (field.length >= 35) renderAttributes["size"] = renderAttributes.get("size", "35");
                    if (field.length < 10) renderAttributes["size"] = renderAttributes.get("size", (field.length + 1).ToString());
                    renderAttributes["value"] = field.value != null ? field.value.ToString() : "";
                    renderAttributes["type"] = "text";
                    string atr = getRenderAttr(renderAttributes);
                    res = this.renderError() + string.Format("<input {0}>", atr);
                }
                else if (field.length >= 150)
                {
                    renderAttributes["cols"] = renderAttributes.get("cols", "35");
                    string atr = getRenderAttr(renderAttributes);
                    res = this.renderError() + string.Format("<textarea {0}>{1}</textarea>", atr, field.value);
                }
                else if (field.length <= 0)     //varchar(MAX)   is -1
                {
                    renderAttributes["cols"] = renderAttributes.get("cols", "55");
                    renderAttributes["rows"] = renderAttributes.get("rows", "5");
                    string atr = getRenderAttr(renderAttributes);
                    res = this.renderError() + string.Format("<textarea {0}>{1}</textarea>", atr, field.value);
                }
            }
            return res;
        }



        

    }
}
