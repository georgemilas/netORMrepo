using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ORM.exceptions;
using ORM.DBFields;
using ORM.db_store.persitence;

namespace ORM.render.RenderControls
{
    public class RenderFK : RenderGeneric
    {
        public RenderFK(GenericField fld)  : base(fld) {}

        public bool useBlankEntity = false;

        //Foreign Key (combo box)
        public override Object renderReadOnly(RenderAttributes renderAttributes)
        {
            return this.renderReadWrite(renderAttributes);
        }

        public override Object renderReadWrite(RenderAttributes renderAttributes)
        {
            DBRelation rel = renderAttributes.fk_as;
            //GenericDatabase db = ORMContext.instance.db;
            GenericDatabase db = field.table.db;

            string fldName = rel.fieldThere_name != null ? rel.fieldThere_name : rel.fieldsThere[0];
            string sql = string.Format("select distinct {0} as {1}, {3} from {2}", db.escape(rel.fieldsThere[0]), db.escape(rel.fieldsHere[0]), rel.tableThere.sqlFromName, db.escape(fldName));
            if (rel.additionalSQLAttributesBuilder != null)
            {
                rel.additionalSQLAttributesBuilder(rel, field);
            }
            if (rel.additionalSQLAttributes != null)
            {
                if (rel.additionalSQLAttributes.from != null)
                {
                    if (rel.additionalSQLAttributes.from.Trim().ToLower().StartsWith("from"))
                    {
                        rel.additionalSQLAttributes.from = rel.additionalSQLAttributes.from.Trim();
                        rel.additionalSQLAttributes.from = rel.additionalSQLAttributes.from.Substring(5);
                    }
                    sql += " " + rel.additionalSQLAttributes.from;
                }
                if (rel.additionalSQLAttributes.where != null)
                {
                    if (!rel.additionalSQLAttributes.where.Trim().ToLower().StartsWith("where"))
                    {
                        rel.additionalSQLAttributes.where = " WHERE " + rel.additionalSQLAttributes.where;
                    }
                    sql += rel.additionalSQLAttributes.where;
                }
                if (rel.additionalSQLAttributes.group != null)
                {
                    if (!rel.additionalSQLAttributes.group.Trim().ToLower().StartsWith("group by"))
                    {
                        rel.additionalSQLAttributes.group = " GROUP BY " + rel.additionalSQLAttributes.group;
                    }
                    sql += rel.additionalSQLAttributes.group;
                }
                if (rel.additionalSQLAttributes.having != null)
                {
                    if (!rel.additionalSQLAttributes.having.Trim().ToLower().StartsWith("having"))
                    {
                        rel.additionalSQLAttributes.having = " HAVING " + rel.additionalSQLAttributes.having;
                    }
                    sql += rel.additionalSQLAttributes.having;
                }
                if (rel.additionalSQLAttributes.order != null)
                {
                    if (!rel.additionalSQLAttributes.order.Trim().ToLower().StartsWith("order by"))
                    {
                        rel.additionalSQLAttributes.order = " ORDER BY " + rel.additionalSQLAttributes.order;
                    }
                    sql += rel.additionalSQLAttributes.order;
                }
            }
            DataTable tb = db.db.getDataTable(sql);


            renderAttributes["name"] = renderAttributes.get("name", field.name);
            string atr = getRenderAttr(renderAttributes);

            string res = this.renderError() + string.Format("<select {0}>\n", atr);
            if (useBlankEntity)
            {
                res += "<option value=\"\"></option>\n";
            }
            foreach (DataRow row in tb.Rows)
            {
                object dbnat = row[0];
                string selected = "";
                if (field.value != null && dbnat != null && dbnat != System.DBNull.Value)
                {
                    //if (dbnat.ToString() == "ROMANIAN") { }
                    selected = dbnat.ToString() == field.value.ToString() ? " selected" : "";
                }
                res += string.Format("<option value=\"{0}\"{1}>{2}</option>\n", row[0], selected, row[1]);
            }
            return res += "</select>\n";
        }
    }
}
