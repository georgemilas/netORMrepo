using System;
using System.Collections.Generic;
using System.Text;

using ORM;
using ORM.DBFields;
using ORM.exceptions;
using EM.Collections;
using System.Data;
using ORM.render.RenderControls;
using System.Web.UI;

namespace ORM.render
{
    public class HTMLListRendererLongDate : HTMLListRenderer
    {
        
        /// <summary>
        /// make sure you provide HTMLListRenderer.table and HTMLListRenderer.name before calling HTMLListRenderer.render
        /// something like this:
        ///    HTMLListRenderer r = new HTMLListRendererLongDate();
        ///    r.name = someTableOfTableRow.name;
        ///    foreach (TableRow tr in someTableOfTableRow)
        ///        r.table.Add(new RenderingProvider(tr));      
        /// </summary>
        public HTMLListRendererLongDate() : base() { }
        public HTMLListRendererLongDate(Table<TableRow> tableData) : base(tableData) { }
        public HTMLListRendererLongDate(Table<TableRow> tableData, Page page): base(tableData, page) { }

        /// <summary>
        /// make sure you also provide HTMLListRenderer.name before calling HTMLListRenderer.render
        /// </summary>
        public HTMLListRendererLongDate(IEnumerable<TableRow> tableData) : base(tableData) { }
        /// <summary>
        /// make sure you also provide HTMLListRenderer.name before calling HTMLListRenderer.render
        /// </summary>
        public HTMLListRendererLongDate(IEnumerable<TableRow> tableData, Page page) : base(tableData, page) { }


        protected override string getValue(FieldRenderControl rc) 
        {
            object val = rc.field.value;
            if (val != null && rc.field.GetType() == typeof(FDatetime))
            {
                try { return DateTime.Parse(val.ToString()).ToString(); }
                catch { return val.ToString(); }
            }
            if (val != null && rc.field.GetType() == typeof(FTimeSpan))
            {
                try { return TimeSpan.Parse(val.ToString()).ToString(); }
                catch { return val.ToString(); }
            }
            if (val == null || val.ToString() == "") return "&nbsp;";

            if (rc.OnValueFormat != null)
            {
                return (string)rc.OnValueFormat(val);
            }
            
            return val.ToString();
        }
        
    }
}
