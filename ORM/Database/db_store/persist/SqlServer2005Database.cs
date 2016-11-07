using System;
using System.Collections.Generic;
using System.Text;


using EM.Collections;
using EM.DB;
using System.Data;

namespace ORM.db_store.persitence
{
    /// <summary>
    /// Backend for MS SQL Server 2005
    /// </summary>
    public class SqlServer2005Database: SQLServerDatabase
    {
        public SqlServer2005Database(SqlServerDBWorker db)
            : base(db)
        { }

        public override SQLStatement limitAndOffset(PKInfo pk, SQLStatement atr)
        {
            // select from ( ... Row_Number() OVER (ORDER BY pk) AS RowsNumberCount ) 
            // where RowsNumberCount >= offset ...
            if (atr.limit > 0 || atr.offset > 0)
            {
                if (pk.Count == 0) pk = this.pk(pk.table);
                if (pk.Count == 0)
                    throw new NotSupportedException("this ORM only suports limit and offset if a PK is set. No pk was found for " + pk.table);
                
                EList<string> pklist = new EList<string>();
                foreach (string f in pk)
                {
                    pklist.Add(string.Format("{0}.[{1}]", pk.table.sqlFromName, f));                  
                }
                //if (!stm.limitOffsetHelper.limitApplied)
                //{
                    string order = "";
                    if (atr.order != null && atr.order.Trim()!="")
                    {
                        order = atr.order;
                        if (order.TrimStart().ToUpper().StartsWith("ORDER BY"))
                        {
                            order = order.Substring(order.IndexOf("ORDER BY")+8);
                        }
                    }
                    else
                    {
                        order = pklist.join(", ");
                    }
                    string fields = atr.fields + string.Format(", Row_Number() OVER (ORDER BY {0}) AS RowsNumberCount", order);
                    atr.limitOffsetHelper.limitApplied = true;
                //}

                if (atr.from != null && atr.from.Trim() != "" && !atr.from.Trim().ToLower().StartsWith("from")) atr.from = "FROM " + atr.from;
                if (atr.where != null && atr.where.Trim() != "" && !atr.where.Trim().ToLower().StartsWith("where")) atr.where = "WHERE " + atr.where;
                if (atr.order != null && atr.order.Trim() != "" && !atr.order.Trim().ToLower().StartsWith("order by")) atr.order = "ORDER BY " + atr.order;
                if (atr.group != null && atr.group.Trim() != "" && !atr.group.Trim().ToLower().StartsWith("group by")) atr.group = "GROUP BY " + atr.group;
                if (atr.having != null && atr.having.Trim() != "" && !atr.having.Trim().ToLower().StartsWith("having")) atr.having = "HAVING " + atr.having;
                string sql = string.Format("SELECT {0} TOP 100 PERCENT {1} {2} {3} {4} {5} {6}", (atr.distinct && !fields.ToUpper().Contains(" DISTINCT ")) ? "DISTINCT" : "", fields, atr.from, atr.where, atr.group, atr.having, atr.order);

                if (atr.limit>0 && atr.offset>0) {
                    atr.customSql = string.Format(@"WITH aaa AS ({0}) Select * from AAA WHERE RowsNumberCount BETWEEN {1} AND {2}", sql, atr.offset, atr.limit + atr.offset);
                }
                else if (atr.limit > 0)
                {
                    atr.customSql = string.Format(@"WITH aaa AS ({0}) Select * from AAA WHERE RowsNumberCount <= {1}", sql, atr.limit);
                }
                else if (atr.offset > 0)
                {
                    atr.customSql = string.Format(@"WITH aaa AS ({0}) Select * from AAA WHERE RowsNumberCount >= {1}", sql, atr.offset);
                }
            }
            return atr;
        }


    }
}
