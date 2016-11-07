using EM.Collections;
using ORM.DBFields;
using ORM.render.RenderControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;

namespace ORM.render
{
    public class HTMLListRenderer : IListRenderer
    {
        /// <summary>
        /// make sure you provide HTMLListRenderer.table and HTMLListRenderer.name before calling HTMLListRenderer.render
        /// something like this:
        ///    HTMLListRenderer r = new HTMLListRenderer();
        ///    r.name = someTableOfTableRow.name;
        ///    foreach (TableRow tr in someTableOfTableRow)
        ///        r.table.Add(new RenderingProvider(tr));      
        /// </summary>
        public HTMLListRenderer()
        {
        }

        public HTMLListRenderer(Table<TableRow> tableData)
        {
            setTableRenderingProvider(tableData);
            this.name = tableData.name;
        }

        public HTMLListRenderer(Table<TableRow> tableData, Page page)
        {
            setTableRenderingProvider(tableData, page);
            this.name = tableData.name;
        }

        /// <summary>
        /// make sure you also provide HTMLListRenderer.name before calling HTMLListRenderer.render
        /// </summary>
        public HTMLListRenderer(IEnumerable<TableRow> tableData)
        {
            setTableRenderingProvider(tableData);
        }

        /// <summary>
        /// make sure you also provide HTMLListRenderer.name before calling HTMLListRenderer.render
        /// </summary>
        public HTMLListRenderer(IEnumerable<TableRow> tableData, Page page)
        {
            setTableRenderingProvider(tableData, page);
        }
        
        private List<RenderingProvider> _table = new List<RenderingProvider>();

        public List<RenderingProvider> table
        {
            get { return _table; }
            set { _table = value; }
        }

        public string name { get; set; }

        protected bool _useAjax;
        public bool useAjax
        {
            get { return this._useAjax; }
            set { this._useAjax = value; }
        }
        
        private void setTableRenderingProvider(IEnumerable<TableRow> tableData)
        {
            List<RenderingProvider> tb = new List<RenderingProvider>();
            foreach (TableRow tr in tableData)
            {
                tb.Add(new RenderingProvider(tr));
            }
            this.table = tb;
        }

        private void setTableRenderingProvider(IEnumerable<TableRow> tableData, Page page)
        {
            List<RenderingProvider> tb = new List<RenderingProvider>();
            foreach (TableRow tr in tableData)
            {
                tb.Add(new RenderingProvider(tr, page));
            }
            this.table = tb;
        }

        public void setFieldFormatter(string fieldName, FieldRenderControl.ValueFormatter formatter)
        {
            foreach (var rp in this.table)
            {
                rp.setFieldFormatter(fieldName, formatter);
            }
        }

        protected DBRelation getFK(GenericField f)
        {
            if (f.table.fk.Count > 0)
            {
                foreach (DBRelation r in f.table.fk)
                {
                    if (f.name == r.fieldsHere[0]) return r;
                }
            }
            return null;
        }

        protected virtual string getValue(FieldRenderControl rc)
        {
            object val = rc.field.value;
            if (val != null && rc.field.GetType() == typeof (FDatetime))
            {
                try
                {
                    if (rc.OnValueFormat != null)
                    {
                        return (string) rc.OnValueFormat(val);
                    }
                    return DateTime.Parse(val.ToString()).ToShortDateString();
                }
                catch
                {
                    return val.ToString();
                }
            }
            if (val != null && rc.field.GetType() == typeof (FTimeSpan))
            {
                try
                {
                    if (rc.OnValueFormat != null)
                    {
                        return (string) rc.OnValueFormat(val);
                    }
                    return TimeSpan.Parse(val.ToString()).ToString();
                }
                catch
                {
                    return val.ToString();
                }
            }
            if (val == null || val.ToString() == "") return "&nbsp;";

            if (rc.OnValueFormat != null)
            {
                return (string) rc.OnValueFormat(val);
            }
            return val.ToString();
        }

        protected string getHTMLValue(TableRow tb, FieldRenderControl rc)
        {
            DBRelation rel = this.getFK(rc.field);
            if (rc.field.value != null && rel != null && rel.fieldThere_name != null)
            {
                SQLStatement atr2 = new SQLStatement(tb.db);
                atr2.from = rel.tableThere.sqlFromName;
                atr2.fields = rel.fieldThere_name;
                atr2.where = tb.db.escape(rel.fieldsThere[0]) + "=" + rc.field.value;
                DataTable r = tb.db.db.getDataTable(tb.db.selectSql(atr2));
                return r.Rows[0][0].ToString();
            }
            else
            {
                return this.getValue(rc);
            }
        }

        public virtual Object render()
        {
            return this.render(null, new EList<ListAction>());
        }

        public virtual Object render(EList<ListAction> actions)
        {
            return this.render(null, actions);
        }

        public virtual Object render(OrderedDictionary<string, string> fields)
        {
            return this.render(fields, new EList<ListAction>());
        }

        public virtual Object render(OrderedDictionary<string, string> fields, EList<ListAction> actions)
        {
            StringBuilder listHTML = new StringBuilder("<table cellspacing=0 id=\"" + this.name + "\">\n");

            if (table.Count > 0)
            {
                //paint header
                listHTML.Append("<tr>\n");

                listHTML = putActionHeader(actions, listHTML, ListAction.Position.Left);

                if (fields != null)
                {
                    foreach (string f in fields.Keys)
                    {
                        listHTML.Append("   <th id=\"" + f + "\" class=\"" + f + "\">" + fields[f] + "</th>\n");
                    }
                }
                else
                {
                    foreach (FieldRenderControl rc in table[0].Values)
                    {
                        if (!rc.renderAttributes.get("style", "").Contains("display: none;"))
                        {
                            string label = rc.renderAttributes.get("label", rc.field.name.Replace("fld_", "").Replace("_", " "));
                            listHTML.Append("   <th id=\"" + rc.field.name + "\" class=\"" + rc.field.name + "\">" + label + "</th>\n");
                        }
                    }
                }

                listHTML = putActionHeader(actions, listHTML, ListAction.Position.Right);

                listHTML.Append("</tr>\n\n");
            }
            
            //paint details
            int cnt = 0;
            foreach (RenderingProvider tb in table)
            {
                listHTML.Append("<tr>");
                cnt += 1;
                putActions(actions, ref listHTML, cnt, tb.table, ListAction.Position.Left);
                if (fields != null)
                {
                    foreach (string f in fields.Keys)
                    {
                        //display even if render attributes style is display: none
                        listHTML.Append("   <td class=\"" + f + "\">");
                        listHTML.Append(this.getHTMLValue(tb.table, tb[tb.table.fields[f]]));
                        listHTML.Append("</td>\n");

                    }
                }
                else
                {
                    foreach (FieldRenderControl rc in tb.Values)
                    {
                        if (!rc.renderAttributes.get("style", "").Contains("display: none;"))
                        {
                            listHTML.Append("   <td class=\"" + rc.field.name + "\">");
                            listHTML.Append(this.getHTMLValue(tb.table, rc));
                            listHTML.Append("</td>\n");
                        }
                    }
                }

                if (tb.table.fields.ContainsKey("UserEditable"))
                {
                    if (tb.table.fields["UserEditable"].value.ToString().ToUpper() == "TRUE")
                    {
                        putActions(actions, ref listHTML, cnt, tb.table, ListAction.Position.Right);
                    }
                }
                listHTML.Append("</tr>\n\n");
            }
            listHTML.Append("</table>\n\n");

            return listHTML.ToString();
        }

        private static StringBuilder putActionHeader(EList<ListAction> actions, StringBuilder listHTML, ListAction.Position position)
        {
            foreach (ListAction ac in actions)
            {
                if (ac.position == position)
                {
                    listHTML.Append("   <th>&nbsp;</th>\n");  //action 
                }
            }
            return listHTML;
        }

        private void putActions(EList<ListAction> actions, ref StringBuilder listHTML, int cnt, TableRow tb, ListAction.Position position)
        {
            foreach (ListAction ac in actions)
            {
                if (ac.position == position)
                {
                    if (ac.url != null)
                    {
                        if (this.useAjax)
                        {
                            listHTML.Append("<td><form method='post' name='" + ac.url + cnt.ToString() + "' action='#' onSubmit=\"callForm('" + ac.url + cnt.ToString() + "', '" + ac.url + "'); return false;\">");  //action 
                        }
                        else
                        {
                            listHTML.Append("<td><form method='post' action='" + ac.url + "'>");  //action 
                        }

                        if (tb.pk.Count > 0)
                        {
                            foreach (string f in tb.pk)
                            {
                                listHTML.Append("<input type='hidden' name='" + f + "' value='" + tb.fields[f].value.ToString() + "'>");
                            }
                        }
                        else
                        {
                            if (ac.url.ToUpper().Contains("COMPANYOPTIONS"))
                            {
                                listHTML.Append("<input type='hidden' name='option_id' value='" + tb.fields["option_id"].value.ToString() + "'>");
                            }
                        }
                        foreach (string f in ac.aditionalFields)
                        {
                            listHTML.Append("<input type='hidden' name='" + f + "' value='" + tb.fields[f].value.ToString() + "'>");
                        }
                        listHTML.Append("<input type='submit' class='button' name='searchGo' value='" + ac.label + "'></form></td>\n");  //action 
                    }
                    else
                    {
                        List<string> vals = new List<string>();
                        ac.aditionalFields.ForEach(new System.Action<string>(delegate(string field) { vals.Add(tb.fields[field].value.ToString()); }));
                        listHTML.Append("<td>" + string.Format(ac.html, vals.ToArray()) + "</td>");
                    }
                }
            }
        }

        public Object navigationLinks(ListPagingData pagingData)
        {
            //paint navigation links
            StringBuilder navLinks = new StringBuilder();

            if (pagingData != null)
            {
                if (pagingData.total > pagingData.limit)
                {
                    int offsetNext = pagingData.offset + pagingData.limit;
                    int offsetPrev = pagingData.offset - pagingData.limit;
                    if (offsetPrev < 0) offsetPrev = 0;

                    int offsetLast = pagingData.limit*(pagingData.total/pagingData.limit);

                    string searchText = pagingData.urlParams;

                    navLinks.Append("<div class='navLinks'>\n");
                    string listUrl = pagingData.url;
                    if (this.useAjax)
                    {
                        string fname = listUrl;
                        if (pagingData.offset > 0)
                        {
                            navLinks.AppendFormat("<a href='#' onClick=\"callUrl('" + fname + "', '{0}?limit={1}&offset={2}&total={3}{4}');\" class='navLink'>First</a>\n", listUrl,
                                pagingData.limit, 0, pagingData.total, searchText);
                            navLinks.AppendFormat("<a href='#' onClick=\"callUrl('" + fname + "', '{0}?limit={1}&offset={2}&total={3}{4}');\" class='navLink'>Previous</a>\n",
                                listUrl, pagingData.limit, offsetPrev, pagingData.total, searchText);
                            if (offsetNext > offsetLast)
                            {
                                navLinks.Append("<span class='navLink'>Next</span>\n");
                                navLinks.Append("<span class='navLink'>Last</span>\n");
                            }
                        }
                        if (offsetNext <= offsetLast)
                        {
                            if (pagingData.offset == 0)
                            {
                                navLinks.Append("<span class='navLink'>First</span>\n");
                                navLinks.Append("<span class='navLink'>Previous</span>\n");
                            }
                            navLinks.AppendFormat("<a href='#' onClick=\"callUrl('" + fname + "', '{0}?limit={1}&offset={2}&total={3}{4}');\" class='navLink'>Next</a>\n", listUrl,
                                pagingData.limit, offsetNext, pagingData.total, searchText);
                            navLinks.AppendFormat("<a href='#' onClick=\"callUrl('" + fname + "', '{0}?limit={1}&offset={2}&total={3}{4}');\" class='navLink'>Last</a>\n", listUrl,
                                pagingData.limit, offsetLast, pagingData.total, searchText);
                        }
                    }
                    else
                    {
                        if (pagingData.offset > 0)
                        {
                            navLinks.AppendFormat("<a href=\"{0}?limit={1}&offset={2}&total={3}{4}\" class='navLink'>First</a>\n", listUrl, pagingData.limit, 0, pagingData.total,
                                searchText);
                            navLinks.AppendFormat("<a href=\"{0}?limit={1}&offset={2}&total={3}{4}\" class='navLink'>Previous</a>\n", listUrl, pagingData.limit, offsetPrev,
                                pagingData.total, searchText);
                            if (offsetNext > offsetLast)
                            {
                                navLinks.Append("<span class='navLink'>Next</span>\n");
                                navLinks.Append("<span class='navLink'>Last</span>\n");
                            }
                        }
                        if (offsetNext <= offsetLast)
                        {
                            if (pagingData.offset == 0)
                            {
                                navLinks.Append("<span class='navLink'>First</span>\n");
                                navLinks.Append("<span class='navLink'>Previous</span>\n");
                            }
                            navLinks.AppendFormat("<a href=\"{0}?limit={1}&offset={2}&total={3}{4}\" class='navLink'>Next</a>\n", listUrl, pagingData.limit, offsetNext,
                                pagingData.total, searchText);
                            navLinks.AppendFormat("<a href=\"{0}?limit={1}&offset={2}&total={3}{4}\" class='navLink'>Last</a>\n", listUrl, pagingData.limit, offsetLast,
                                pagingData.total, searchText);
                        }
                    }
                    navLinks.Append("</div>\n\n");
                }
            }

            return navLinks.ToString();
        }
    }
}