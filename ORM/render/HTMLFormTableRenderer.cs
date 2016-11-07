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
using EM.DB;
using System.Data.SqlClient;

namespace ORM.render
{
    public class HTMLFormTableRenderer : RenderingProvider, IFormRenderer
    {
        public HTMLFormTableRenderer() : base() { }
        public HTMLFormTableRenderer(Page page, TableRow table) : base(table, page) { }

        public string getRenderAttr(RenderAttributes renderAttributes) 
        {
            if (renderAttributes==null) return "";
            return renderAttributes.ToString();
        }

        //THE WEB FORM 
        /// <summary>
        /// return  <code>
        ///         --read only: 
        ///             &lt;div id='|table name|' class='ormForm'>&lt;table><br>
        ///         <br>
        ///         --read write: <br>
        ///             &lt;form id='|table name|' class='ormForm'>&lt;table><br>
        ///             &lt;tr>&lt;td colspan=2 class='formInfo'>All fields marked with &lt;em class='require'>*&lt;/em> are required
        ///         <br>
        ///         --for all:    
        ///         &lt;tr id='|tableName_fieldName|'><br>
        ///             &lt;td class='dbFieldLabel'>  ... field name<br>
        ///             &lt;td>.... rendered field control (see field.renderControl)
        ///         </code>
        /// </summary>
        public override Object render(RenderAttributes renderAttributes)
        {
            if (renderAttributes == null) renderAttributes = new RenderAttributes();
            renderAttributes["method"] = renderAttributes.get("method", "POST");
            renderAttributes["name"] = renderAttributes.get("name", table.dbObjectName.table);
            renderAttributes["id"] = renderAttributes.get("id", table.dbObjectName.table);
            renderAttributes["class"] = renderAttributes.get("class", "ormForm");

            string atr = getRenderAttr(renderAttributes);
            StringBuilder frm = new StringBuilder(string.Format("<form {0}>\n", atr));
            frm.Append("<table cellspacing=\"0\" cellpadding=\"0\">\n");
            foreach (FieldRenderControl rc in this.Values)
            {
                if (!rc.field.isComputed && rc.field.isRequired)
                {
                    frm.Append("<tr><td colspan=2 class='formInfo'>All fields marked with <em class='require'>*</em> are required</td></tr>\n");
                    break;
                }
            }
            foreach(GenericField f in this.table.fields.Values) 
            {
                string frender = "";

                if ( (renderAttributes.renderFields != null &&  !renderAttributes.renderFields.ContainsKey(f.name)) 
                     ||
                     (renderAttributes.skipRenderFields != null && renderAttributes.skipRenderFields.Contains(f.name))
                   )
                {
                    if (!this.table.pk.Contains(f.name) || f.oldValueSafe == null) continue;
                    else
                    {
                        frender = string.Format("<input type=\"hidden\" name=\"hidden_{0}\" value=\"{1}\">", f.name, f.oldValueSafe);
                        if (frm.Length > 0)
                        {
                            frm = frm.Remove(frm.Length - 11, 11);  // "</td></tr>\n"
                            frm.AppendFormat("{0}</td></tr>\n", frender);
                        }
                        else
                        {
                            frm.AppendFormat("{0}\n", frender);
                        }
                        continue;                                
                    }
                }

                frender = (string)this[f].render(); 

                if (f.table.pk.Contains(f.name))
                {
                    frender += string.Format("<input type=\"hidden\" name=\"hidden_{0}\" value=\"{1}\">", f.name, f.oldValueSafe);
                }
                string label = (string)this[f].renderLabel(renderAttributes);
                frm.AppendFormat("<tr id=\"{0}_{1}\"><td>{2}</td><td>{3}</td></tr>\n", table.dbObjectName.table, f.name, label, frender);

            }
            frm.Append("</table>\n");

            string fname = renderAttributes["name"];

            string submit = "";
            foreach (string f in this.table.pk)
            {   //add a delete button if data in database ?
                if (table.fields[f].oldValueSafe != null)
                {
                    //be friendly for Ajax submit so that it sends the correct value only for the button that was actualy pressed
                    //submit = "<input type=\"submit\" class=\"button\" name=\"Save\" value=\"Save\" onClick=\"this.value='Save';document.forms['" + fname + "']['Delete'].value='';return true;\">\n";
                    submit = "<input type=\"submit\" class=\"button\" name=\"Save\" value=\"Save\" onClick=\"this.value='Save';return true;\">\n";
                    submit += "<input type=\"hidden\" name=\"isInDB\" value=\"yes\">\n";
                    //submit += "<input type=\"submit\" class=\"button\" name=\"Delete\" value=\"Delete\" onClick=\"this.value='Delete';document.forms['" + fname + "']['Save'].value=''; if (confirm('Are you sure you want to delete this?\\nThis action can not be undone.')) { return true; } else { document.forms['" + fname + "']['Save'].value='Save';return false;}\">\n";
                    break;
                }
            }
            if (submit == "")
            {
                submit = "<input type=\"submit\" class=\"button\" name=\"Save\" value=\"Save\">\n";
            }
            frm.Append(submit);
            frm.Append("</form>\n");
            return frm.ToString();
        }


        /// <summary>
        /// - if page has form object isInDB==yes 
        ///     - then first set fields with data from db (using PK from the page form)
        /// - set object field with values found in page form data
        /// </summary>
        public override BusinessLogicError setTableFromWebForm()
        {
            this.setPageToFieldControls();
            BusinessLogicError ret = null;
            if (this.page.Request.Params["isInDB"] == "yes")
            {
                if (this.table is TableRowStoredProcBased)
                {
                    foreach (string pkf in this.table.pk)
                    {
                        this.table.fields[pkf].oldValue = this.page.Request.Params["hidden_" + pkf];                        
                    }
                }

                if (this.table is TableRowDynamicSQL)
                {
                    EList<string> where = new EList<string>();
                    DBParams p = new DBParams();
                    foreach (string pkf in this.table.pk)
                    {
                        where.Add(string.Format("{0} = @{1}", this.table.db.escape(pkf), ORMContext.fixName(pkf)));
                        p.Add(new DBParam("@" + ORMContext.fixName(pkf), this.page.Request.Params["hidden_" + pkf]));
                    }
                    SQLStatement atr = new SQLStatement(this.table);
                    atr.where = where.join(" and ");

                    try { ((TableRowDynamicSQL) this.table).setFromDB(atr, p); }
                    catch (BusinessLogicError e) { ret = e; }
                    catch (SqlException e) { }
                    catch (Exception e) { }
                }
            }

            foreach (FieldRenderControl rc in this.Values)
            {
                rc.readPageValue();
            }
            return ret;
        }

        

    }
}
