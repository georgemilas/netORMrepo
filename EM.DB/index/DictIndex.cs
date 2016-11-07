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
    public class DictIndex
    {
        public DataTable table;
        protected string fieldName { get; set; }
        private DictLookup<string, DataRow> inner;

        public DictIndex()
        {
            this.table = null;
        }
        public DictIndex(DataTable tbl, string fieldName): this(tbl, fieldName, false) { }
        public DictIndex(DataTable tbl, string fieldName, bool toUpper)
        {
            this.table = tbl;
            this.fieldName = fieldName;
            //var en = from row in tbl.AsEnumerable() select new DataRowFieldGetter(row);
            this.inner = new DictLookup<string, DataRow>(tbl.AsEnumerable(), new DictKeyProvider<DataRow>(fieldName, toUpper, new DataRowFieldGetter()));             
        }

        public void addRow(DataRow row)
        {
            //we use strings for keys so that we can match numbers for example we may have a double and a decimal, 
            //both the same number but they would not match otherwise 
            string key = this.inner.KeyProvider.GetKey(row);
            this.inner.DataStore.setdefault(key, new EList<DataRow>()).Add(row);            
        }

        public DataTable Select(object value)
        {
            if (this.table != null)
            {
                DataTable t = this.table.Clone();
                t.TableName = this.table.TableName;

                foreach (var r in this.inner.Select(new[] {value}))
                {
                    t.ImportRow(r);
                }
                return t;
            }
            else
            {
                return new DataTable();
            }
        }

        public DataRow[] SelectDataRow(object value)
        {
            if (this.table != null)
            {
                return this.inner.Select(new[] {value}).ToArray();
            }
            else
            {
                return new DataRow[] {};
            }
        }       
    }


}
