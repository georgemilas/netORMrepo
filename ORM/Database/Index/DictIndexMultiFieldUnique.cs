using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace ORM.Index
{
    /// <summary>
    /// usage:
    ///     DictIndex idx = new DictIndex(orders, "fld_cust_id")
    //      DataTable myOrders = idx.Select(11234)  //return all orders for customer 11234
    /// </summary>
    public class DictIndexMultiFieldUnique<T> : EM.DB.Index.DictIndexMultiFieldUnique where T : TableRow
    {
        public ITable<T> original { get; set; }
        public DictIndexMultiFieldUnique(ITable<T> tbl, IEnumerable<string> fieldNames): this(tbl, fieldNames, false) { }
        public DictIndexMultiFieldUnique(ITable<T> tbl, IEnumerable<string> fieldNames, bool toUpper) : base(tbl.adoDataTable, fieldNames, toUpper) { original = tbl; }
        
        public T FindInstance(ORMContext cx, IEnumerable<object> values)
        {
            DataRow dr = Find(values);
            if (dr != null)
            {
                T t = TableRow.getInstance<T>(cx);
                t.setFromDataRow(dr);
                return t;
            }
            return default(T);
        }
    }
}
