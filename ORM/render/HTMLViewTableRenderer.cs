using System;
using System.Collections.Generic;
using System.Text;

using ORM;
using ORM.DBFields;
using ORM.exceptions;
using EM.Collections;
using EM.DB;
using System.Data;
using ORM.render.RenderControls;
using System.Web.UI;

namespace ORM.render
{
    public class HTMLViewTableRenderer : RenderingProvider, IFormRenderer
    {
        public HTMLViewTableRenderer() : base() { }
        public HTMLViewTableRenderer(Page page, TableRow table) : base(table, page) 
        {
            foreach (FieldRenderControl c in this.Values)
            {
                c.isReadOnly = true;
            }
        }

        public string getRenderAttr(RenderAttributes renderAttributes) 
        {
            if (renderAttributes==null) return "";

            EList<string> res = new EList<string>();
            foreach(string k in renderAttributes.Keys) 
            {
                res.Add(string.Format("{0}=\"{1}\"", k, renderAttributes[k]));
            }
            return res.join(" ");
        }

        //THE DIV
        /// <summary>
        /// return &lt;div id='|table name|' class='ormForm'>&lt;table><br>
        ///         &lt;tr id='|tableName_fieldName|'><br>
        ///             &lt;td class='dbFieldLabel'>  ... field name<br>
        ///             &lt;td>.... rendered field control (see field.renderControl)
        /// </summary>
        public override Object render(RenderAttributes renderAttributes)
        {
            if (renderAttributes == null) renderAttributes = new RenderAttributes();
            renderAttributes["id"] = renderAttributes.get("id", table.dbObjectName.table);
            renderAttributes["class"] = renderAttributes.get("class", "ormForm");

            string atr = getRenderAttr(renderAttributes);
            StringBuilder frm = new StringBuilder(string.Format("<div {0}>\n", atr));
            frm.Append("<table cellspacing=\"0\" cellpadding=\"0\">\n");
            foreach(FieldRenderControl rc in this.Values) 
            {
                string frender = "";

                if ( (renderAttributes.renderFields != null &&  !renderAttributes.renderFields.ContainsKey(rc.field.name)) 
                     ||
                     (renderAttributes.skipRenderFields != null && renderAttributes.skipRenderFields.Contains(rc.field.name))
                   )
                {
                     continue;                    
                }
                if (rc.renderAttributes.get("style", "").Contains("display: none;")) { continue; }

                rc.isReadOnly = true;
                frender = (string)rc.render();

                string label = rc.getFieldLabel(renderAttributes);
                frm.AppendFormat("<tr id=\"{0}_{1}\"><td class=\"dbFieldLabel\">{2}</td><td>{3}</td></tr>\n", this.table.dbObjectName.table, rc.field.name, label, frender);

            }
            frm.Append("</table>\n");
            frm.Append("</div>\n");
            return frm.ToString();
        }

    }
}
