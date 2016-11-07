using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EM.Collections;
using EM.DB.Index.Generic;

namespace EM.DB.Index
{
    /// <summary>
    /// usage:
    ///     DictIndex idx = new DictIndex(orders, "fld_cust_id")
    //      DataTable myOrders = idx.Select(11234)  //return all orders for customer 11234
    /// </summary>
    public class DictIndexMultiFieldUnique
    {
        public DataTable table;
        private GenericLookup<MultiFieldDictKey, DataRow> inner;

        public DictIndexMultiFieldUnique(DataTable tbl, IEnumerable<string> fieldNames) : this(tbl, fieldNames, false) { }
        public DictIndexMultiFieldUnique(DataTable tbl, IEnumerable<string> fieldNames, bool toUpper)
        {
            this.table = tbl;
            this.inner = new GenericLookup<MultiFieldDictKey, DataRow>(tbl.AsEnumerable(), new UniqueMultiFieldDictKeyProvider<DataRow>(fieldNames, toUpper, new DataRowFieldGetter()));
        }

        /// <summary>
        /// returns null if item is not found and throws exception if more than one item exists
        /// </summary>
        public DataRow Find(IEnumerable<object> values)
        {
            return this.inner.Select(values).SingleOrDefault();
        }
    }
}
