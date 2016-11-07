using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    public class RenderDateTime : RenderGeneric
    {
        public RenderDateTime(FDatetime fld) : base(fld) { }

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            if (field.value != null)
            {
                if (field.value is DateTime) { renderAttributes["value"] = ((DateTime)field.value).ToShortDateString(); }
                else
                {
                    try { renderAttributes["value"] = DateTime.Parse(field.value.ToString()).ToShortDateString(); }
                    catch { renderAttributes["value"] = field.value.ToString(); }
                }
            }
            else renderAttributes["value"] = "";
            if (field.isComputed)
            {
                return string.Format("<label>{0}</label>", renderAttributes["value"]);
            }

            renderAttributes["name"] = field.name;
            renderAttributes["type"] = "text";
            string atr = getRenderAttr(renderAttributes);
            return this.renderError() + string.Format("<input {0}>", atr);
        }

        public override Object renderReadOnly(RenderAttributes renderAttributes)
        {
            if (field.value != null)
            {
                if (field.value is DateTime) { renderAttributes["value"] = ((DateTime)field.value).ToShortDateString(); }
                else { renderAttributes["value"] = field.value.ToString(); }
            }
            else renderAttributes["value"] = "";

            if (!renderAttributes.get("style", "").Contains("display: none;"))
            {
                return string.Format("<label>{0}</label>", renderAttributes["value"]);
            }
            return "";
        }

        

    }
}
