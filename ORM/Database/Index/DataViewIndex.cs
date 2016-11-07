using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace ORM.Index
{ 
    /// <summary>
    /// usage:
    ///     DataViewIndex idx = new DataViewIndex(orders, "fld_cust_id ASC, fld_invoice DESC")
    //      DataTable myLatestOrders = idx.Select("fld_cust_id = 11234 and fld_invoice > 423455")
    /// </summary>
    public class DataViewIndex<T> : EM.DB.Index.DataViewIndex where T : TableRow
    {
        public ITable<T> original { get; set; }

        public DataViewIndex(ITable<T> tbl) : base(tbl.adoDataTable) { original = tbl; }
        public DataViewIndex(ITable<T> tbl, string sortExpression) : base(tbl.adoDataTable, sortExpression) { original = tbl; }

        public Table<T> SelectInstances(ORMContext cx, string filterExpression)
        {
            DataTable tb = Select(filterExpression);
            return TableRow.getInstancesFromDataTable<T>(cx, tb);                     
        }
    }
    
}
