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
    public class DictIndexMultiField<T> : EM.DB.Index.DictIndexMultiField where T: TableRow
    {
        public ITable<T> original { get; set; }
        public DictIndexMultiField(ITable<T> tb, IEnumerable<string> fieldNames) : this(tb, fieldNames, false) { }
        public DictIndexMultiField(ITable<T> tb, IEnumerable<string> fieldNames, bool toUpper) : base(tb.adoDataTable, fieldNames, toUpper) { original = tb; }
        
        
        public Table<T> SelectInstances(ORMContext cx, IEnumerable<object> values)
        {
            DataTable tb = Select(values);
            return TableRow.getInstancesFromDataTable<T>(cx, tb);            
        }

    }


}
