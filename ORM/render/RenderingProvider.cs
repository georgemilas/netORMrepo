using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using ORM.DBFields;
using ORM.render.RenderControls;
using System.Web.UI;
using ORM.exceptions;


namespace ORM.render
{
    public class RenderingProvider: OrderedDictionary<GenericField, FieldRenderControl>, IRenderingProvider 
    {
        private TableRow _table;
        public TableRow table
        {
            get { return _table; }
            set 
            { 
                _table = value;
                foreach (GenericField f in _table.fields.Values)
                {
                    this[f] = this.getDefaultRenderControl(f);
                }
                setPageToFieldControls();
            }
        }

        private Page _page;
        public Page page
        {
            get { return _page; }
            set 
            { 
                _page = value;
                setPageToFieldControls();                
            }
        }

        public new FieldRenderControl this[GenericField k]
        {
            get
            {
                return base[k];
            }
            set
            {
                if (this.page != null)
                {
                    value.page = this.page;
                }
                base[k] = value;                                
            }
        }

        protected void setPageToFieldControls()
        {
            if (this._table != null && this._page != null)
            {
                foreach (FieldRenderControl rc in this.Values)
                {
                    rc.page = this.page;
                }
            }
        }

        private RenderAttributes _renderAttributes;
        public RenderAttributes renderAttributes
        {
            get { return _renderAttributes; }
            set { _renderAttributes = value; }
        }


        public RenderingProvider() { }
        public RenderingProvider(TableRow table) 
        {
            this.table = table;   
        }
        public RenderingProvider(TableRow table, Page page)
            : this(table) 
        {
            this.page = page;
        }
        public void setFieldFormatter(string fieldName, FieldRenderControl.ValueFormatter formatter)
        {

            GenericField fld = this.table.fields[fieldName];
            this[fld].OnValueFormat = formatter;
        }
        public FieldRenderControl getDefaultRenderControl(GenericField field)
        {
            if (field is FChar)
            {
                return new RenderText((FChar) field);            
            }
            else if (field is FBoolean)
            {
                return new RenderBool((FBoolean)field);
            }
            else if (field is FDatetime)
            {
                return new RenderDateTime((FDatetime)field);
            }
            else if (field is FNumber)
            {
                return new RenderNumber((FNumber)field);
            }
            else 
            {
                return new RenderGeneric(field);
            }
        }


        public virtual Object render() { return this.render(null); }
        public virtual Object render(RenderAttributes renderAttributes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FieldRenderControl rc in this.Values)
            {
                sb.AppendLine((string)rc.render());
            }
            return sb.ToString();
        }

        /// <summary>
        /// set object field with values found in page form data
        /// </summary>
        public virtual BusinessLogicError setTableFromWebForm()
        {
            this.setPageToFieldControls();
            foreach (FieldRenderControl rc in this.Values)
            {
                rc.readPageValue();
            }
            return null;
        }

    }
}
