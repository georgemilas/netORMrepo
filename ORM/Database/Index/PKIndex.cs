using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace ORM.Index
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
    public class PKIndex<T> : EM.DB.Index.PKIndex where T : TableRow
    {
        public ITable<T> original { get; set; }

        public PKIndex(ITable<T> tb, string pk) : this(tb, new string[] { pk }) { }
        public PKIndex(ITable<T> tb, string[] pk) : base(tb.adoDataTable, pk) { original = tb; }
        public PKIndex(ITable<T> tb, DataColumn[] pk) : base(tb.adoDataTable, pk) { original = tb; }

        public Table<T> SelectInstances(ORMContext cx, string filterExpression)
        {
            DataTable tb = Select(filterExpression);
            return TableRow.getInstancesFromDataTable<T>(cx, tb);
        }

        public Table<T> SelectInstances(ORMContext cx, string filterExpression, string sort)
        {
            DataTable tb = Select(filterExpression, sort);
            return TableRow.getInstancesFromDataTable<T>(cx, tb);
        }
        
        /// <summary>
        /// may return null (used to throw System.Data.MissingPrimaryKeyException)
        /// </summary>        
        public T FindInstance(ORMContext cx, object key)
        {
            DataRow dr = Find(key);
            if (dr == null) { return null; }

            T t = TableRow.getInstance<T>(cx);
            t.setFromDataRow(dr);
            return t;            
        }

        /// <summary>
        /// may return null (used to throw System.Data.MissingPrimaryKeyException)
        /// </summary>        
        public T FindInstance(ORMContext cx, object[] keys)
        {
            DataRow dr = Find(keys);
            if (dr == null) { return null; }

            T t = TableRow.getInstance<T>(cx);
            t.setFromDataRow(dr);
            return t;            
        }

    }


}
