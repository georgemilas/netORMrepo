using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace EM.DB.Index
{

    /// <summary> 
    /// usage:
    ///     PKIndex idx = new PKIndex(orders, "fld_id")    -- fld_id is unique in the table
    ///     DataRow r = idx.Find(345678)     //ther order number 345678
    /// 
    ///     PKIndex idx = new PKIndex(orders, new string[] {"fld_cust_id", "fld_invoice"})      
    ///     //the (cust_id, invoice) tuple is unique in the table
    ///     DataTable myOrders = idx.Select("fld_cust_id = 11234 and fld_invoice > 423455")
    /// </summary>
    public class PKIndex
    {
        public DataTable table;
         
        public PKIndex(DataTable tb, string pk)
            : this(tb, new string[] { pk })
        { }

        public PKIndex(DataTable tb, string[] pk)
        {
            if (tb != null && tb.Columns.Count > 0)
            {
                DataColumn[] pkc = new DataColumn[pk.Length];
                for (int i = 0; i < pk.Length; i++)
                {
                    //Console.WriteLine("PK -> table: {0}, arr: {1}, column: {2}", tb.TableName, pk[i], tb.Columns[pk[i]].ColumnName);
                    pkc[i] = tb.Columns[pk[i]];
                }

                tb.PrimaryKey = pkc;
            }
            this.table = tb;
        }

        public PKIndex(DataTable tb, DataColumn[] pk)
        {
            tb.PrimaryKey = pk;
            this.table = tb;
        }

        public DataTable Select(string filterExpression)
        {
            return SqlServerDBWorker.getDataTable(this.table.Select(filterExpression), this.table);
        }

        public DataTable Select(string filterExpression, string sort)
        {
            return SqlServerDBWorker.getDataTable(this.table.Select(filterExpression, sort), this.table);
        }
        
        public DataRow[] SelectDataRow(string filterExpression)
        {
            return this.table.Select(filterExpression);
        }

        public DataRow[] SelectDataRow(string filterExpression, string sort)
        {
            return this.table.Select(filterExpression, sort);
        }

        /// <summary>
        /// may return null (used to throw System.Data.MissingPrimaryKeyException)
        /// </summary>               
        public DataRow Find(object key)
        {
            return this.table.Rows.Find(key);
        }

        public DataRow Find(object[] keys)
        {
            return this.table.Rows.Find(keys);
        }

    }


}
