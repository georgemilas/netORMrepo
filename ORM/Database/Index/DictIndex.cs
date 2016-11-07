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
    public class DictIndex<T> : EM.DB.Index.DictIndex where T: TableRow
    {
        public ITable<T> original { get; set; }

        public DictIndex(ITable<T> tbl, string fieldName): this(tbl, fieldName, false) { }
        public DictIndex(ITable<T> tbl, string fieldName, bool toUpper) : base(tbl.adoDataTable, fieldName, toUpper) { original = tbl; }
        
        public Table<T> SelectInstances(ORMContext cx, object value)
        {
            DataTable tb = Select(value);
            return TableRow.getInstancesFromDataTable<T>(cx, tb);            
        }        
    }


}
