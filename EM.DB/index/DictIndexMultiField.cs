using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EM.Collections;
using System.Linq;
using EM.DB.Index.Generic;


namespace EM.DB.Index
{
    public class DataRowFieldGetter: ILookupFieldGetter<DataRow>
    {
        public object GetField(DataRow r, string fieldName)
        {
            return r[fieldName];
        }
    }

    public class DictIndexMultiField
    {
        public DataTable table;
        private GenericLookup<MultiFieldDictKey, DataRow> inner;

        public DictIndexMultiField()
        {
            this.table = null;
        }
        public DictIndexMultiField(DataTable tbl, IEnumerable<string> fieldNames): this(tbl, fieldNames, false) { }
        public DictIndexMultiField(DataTable tbl, IEnumerable<string> fieldNames, bool toUpper)
        {
            this.table = tbl;
            this.inner = new GenericLookup<MultiFieldDictKey, DataRow>(tbl.AsEnumerable(), new MultiFieldDictKeyProvider<DataRow>(fieldNames, toUpper, new DataRowFieldGetter()));            
        }
        
        public DataTable Select(IEnumerable<object> values)
        {
            if (this.table != null)
            {
                DataTable t = this.table.Clone();
                t.TableName = this.table.TableName;

                foreach (var r in this.inner.Select(values))
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

        public DataRow[] SelectDataRow(IEnumerable<object> values)
        {
            if (this.table != null)
            {
                return this.inner.Select(values).ToArray();
            }
            else
            {
                return new DataRow[] { };
            }
        }
        
    }
   
}
