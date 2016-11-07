using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.DBFields;
using ORM.exceptions;
using System.Web.UI;

namespace ORM.render.RenderControls
{
    public abstract class FieldRenderControl
    {
        public GenericField field;
        public RenderAttributes renderAttributes;

        public delegate Object ValueFormatter(Object value);
        public ValueFormatter OnValueFormat;
        //public event ValueFormatter OnValueFormat;

        public FieldRenderControl(GenericField fld)
        {
            this.field = fld;
            this.renderAttributes = new RenderAttributes();
        }

        private Page _page;
        public virtual Page page
        {
            get { return _page; }
            set { _page = value; }
        }


        private bool _isReadOnly = false;
        public bool isReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }


        public abstract Object renderReadWrite(RenderAttributes renderAttributes);
        public abstract Object renderReadOnly(RenderAttributes renderAttributes);
        public abstract Object renderRequiredAsterix(RenderAttributes renderAttributes);
        /// <summary>
        /// this should use getFieldLabel internaly to get the value of the label and then 
        /// render that value 
        /// </summary>
        public abstract Object renderLabel(RenderAttributes renderAttributes);
        public abstract string getFieldLabel(RenderAttributes renderAttributes);

        public virtual Object renderRequiredAsterix() { return this.renderRequiredAsterix(combineRenderAttributes(new RenderAttributes())); }
        public virtual Object renderRequiredAsterix(string atrDictLiteral) { return this.renderRequiredAsterix(this.combineRenderAttributes(RenderAttributes.fromStr(atrDictLiteral))); }
        public virtual Object renderLabel() { return this.renderLabel(combineRenderAttributes(new RenderAttributes())); }
        public virtual Object renderLabel(string atrDictLiteral) { return this.renderLabel(this.combineRenderAttributes(RenderAttributes.fromStr(atrDictLiteral))); }
        
        
        public virtual Object renderMain(RenderAttributes renderAttributes)
        {
            if (renderAttributes == null) renderAttributes = new RenderAttributes();
            if (this.isReadOnly)
            {
                return this.renderReadOnly(renderAttributes);
            }
            else
            {
                return this.renderReadWrite(renderAttributes);
            }

        }


        ///////////////////////////////////////////////////////////////////////////////////////
        //////  PUBLIC INTERFACE

        /// <summary>
        /// render using current field render attributes
        /// </summary>
        public virtual Object render() { return this.render(new RenderAttributes()); }
        /// <summary>
        /// - same as render(RenderAttributes.fromStr(atrDictLiteral))
        /// </summary>
        public virtual Object render(string atrDictLiteral) { return this.render(RenderAttributes.fromStr(atrDictLiteral)); }
        /// <summary>
        /// render using current field render attributes (this.renderAttributes) combined with custom given renderAttributes
        /// </summary>
        public virtual Object render(RenderAttributes atr)
        {
            return this.renderMain(this.combineRenderAttributes(atr));
        }


        ///////////////////////////////////////////////////////////////////////////////////////
        //////  HELPERS

        public string getRenderAttr(RenderAttributes renderAttributes)
        {
            if (renderAttributes == null) return "";
            return renderAttributes.ToString();
        }

        //VALIDATION ERRORS
        public virtual string renderError()
        {
            string err = "";
            if (this.field.validationErrors.Count > 0)
            {
                err += "<div class=\"validationError\">";
                foreach (ValidationException e in this.field.validationErrors)
                {
                    err += e.Message + "<br>";
                }
                if (err.EndsWith("<br>")) err = err.Substring(0, err.Length - 4);
                err += "</div>";
            }
            return err;
        }

        protected RenderAttributes combineRenderAttributes(RenderAttributes renderAttributes)
        {
            if (renderAttributes == null) renderAttributes = new RenderAttributes();
            if (renderAttributes.fk_as == null)
            {
                if (this.renderAttributes.fk_as == null && this.field.table.fk.Count > 0)
                {
                    foreach (DBRelation rel in this.field.table.fk)
                    {
                        if (rel.fieldsHere[0] == this.field.name)  //one key relation
                        {
                            this.renderAttributes.fk_as = rel;
                            break;
                        }
                    }
                }
                renderAttributes.fk_as = this.renderAttributes.fk_as;
            }
            return RenderAttributes.combine(renderAttributes, this.renderAttributes);
        }

        public virtual void readPageValue()
        {
            string val = this.page.Request.Params[this.field.name];
            val = (val != null && val.Trim() == "") ? null : val;
            this.field.value = val;
        }

    }
}
