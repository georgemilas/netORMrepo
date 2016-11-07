using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;
using EM.DB.Index.Generic;
using System.Linq;

namespace EM.DB.Index
{
    /// <summary>
    /// usage:
    ///     DictIndexUnique idx = new DictIndex(orders, "fld_order_id")
    //      DataRow myOrder = idx.Find(11234)  //return order 11234
    /// </summary>
    public class DictIndexUnique
    {
        private GenericLookup<string, DataRow> inner;

        public DictIndexUnique(DataTable tbl, string fieldName) : this(tbl, fieldName, false) { }
        public DictIndexUnique(DataTable tbl, string fieldName, bool toUpper)
        {
            this.inner = new GenericLookup<string, DataRow>(tbl.AsEnumerable(), new UniqueDictKeyProvider<DataRow>(fieldName, toUpper, new DataRowFieldGetter()));
        }

        /// <summary>
        /// returns null if item is not found and throws exception if more than one item exists
        /// </summary>
        public DataRow Find(object value)
        {
            return this.inner.Select(new[] { value }).SingleOrDefault();
        }

    }


}
