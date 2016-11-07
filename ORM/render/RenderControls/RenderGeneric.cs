using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.DBFields;

namespace ORM.render.RenderControls
{
    /// <summary>
    /// render as &lt;textarea cols=35
    /// </summary>
    public class RenderGeneric : FieldRenderControl
    {
        public RenderGeneric(GenericField fld) : base(fld) { }

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
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
                renderAttributes["cols"] = renderAttributes.get("cols", "35");
                string atr = getRenderAttr(renderAttributes);
                res = this.renderError() + string.Format("<textarea {0}>{1}</textarea>", atr, field.value.ToString() );                
            }
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
                    else val = field.value.ToString();
                }
                return string.Format("<label>{0}</label>", val);
            }
            return "";
        }

        public override Object renderRequiredAsterix(RenderAttributes renderAttributes)
        {
            if ( this.field.isRequired ) 
            {
                return "<span class='fldRequired'>*</span>"; 
            }
            return "";
        }

        public override Object renderLabel(RenderAttributes renderAttributes)
        {
            string label = getFieldLabel(renderAttributes);
            return string.Format("<label class=\"dbFieldLabel\">{0}</label>{1}", label, (string)renderRequiredAsterix(renderAttributes));            
        }

        public override string getFieldLabel(RenderAttributes renderAttributes)
        {
            string label = "";
            if (renderAttributes.ContainsKey("label"))
            {
                //the label was provided to the actual render control
                label = renderAttributes["label"];
            }
            else if (renderAttributes.renderFields != null && renderAttributes.renderFields.ContainsKey(this.field.name))
            {
                //the label was provided to the table renderFields dictionary 
                label = renderAttributes.renderFields[this.field.name];
            }
            else
            {
                //default label 
                label = this.field.name.Replace("fld_", "").Replace("_", " ");
            }

            if (renderAttributes.get("style", "") == "display: none;")
            {
                label = "";
            }
            return label;
        } 
    }
}
