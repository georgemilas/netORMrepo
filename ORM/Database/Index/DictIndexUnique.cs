using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace ORM.Index
{
    /// <summary>
    /// usage:
    ///     DictIndexUnique idx = new DictIndex(orders, "fld_order_id")
    //      DataRow myOrder = idx.Find(11234)  //return order 11234
    /// </summary>
    public class DictIndexUnique<T> : EM.DB.Index.DictIndexUnique where T : TableRow
    {
        public ITable<T> original { get; set; }

        public DictIndexUnique(ITable<T> tbl, string fieldName) : this(tbl, fieldName, false) { }
        public DictIndexUnique(ITable<T> tbl, string fieldName, bool toUpper) : base(tbl.adoDataTable, fieldName, toUpper) { original = tbl; }

        public T FindInstance(ORMContext cx, object value)
        {
            DataRow dr = Find(value);
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
