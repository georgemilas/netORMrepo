using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;

namespace EM.DB.Index
{ 
    /// <summary>
    /// usage:
    ///     DataViewIndex idx = new DataViewIndex(orders, "fld_cust_id ASC, fld_invoice DESC")
    //      DataTable myLatestOrders = idx.Select("fld_cust_id = 11234 and fld_invoice > 423455")
    /// </summary>
    public class DataViewIndex
    {
        private DataView view;   //DataView creates an index internaly so our job is just to wrapp it

        public DataViewIndex(DataTable tbl)
        {
            this.view = new DataView(tbl);
        }

        public DataViewIndex(DataTable tbl, string sortExpression) 
        {
            this.view = new DataView(tbl);
            this.view.Sort = sortExpression;
        }

        /// <summary>
        /// Adds a new empty row to the view/table.
        /// You must call EndEdit on the new row when you are done seeting values into it.
        /// </summary>
        public DataRowView AddNewRow()
        {
            DataRowView drw = this.view.AddNew();
            return drw;            
        }

        public void AddNewRow(DataRow row)
        {
            DataRowView drw = this.AddNewRow();
            foreach (DataColumn c in this.view.Table.Columns)
            {
                drw.Row[c.ColumnName] = row[c.ColumnName];
            }
            drw.EndEdit();
        }

        public override string ToString()
        {
            string tbl = this.view.Table.TableName;
            return String.Format("FROM {0} ORDER BY {1}", tbl, this.view.Sort);
        }

        public DataTable Select(string filterExpression)
        {   
            //for consistency returning DataTable with the cost ? of an extra loop to convert      
            this.view.RowFilter = filterExpression;   
            return this.view.ToTable();               
        }
        
        public DataRow[] SelectDataRow(string filterExpression)
        {
            this.view.RowFilter = filterExpression;
            DataTable t = this.view.ToTable();
            if (t.Rows.Count > 0)
            {
                DataRow[] res = new DataRow[t.Rows.Count];
                for (int i=0; i< t.Rows.Count; i++)
                {
                    res[i] = t.Rows[i];
                }
                return res;
            }
            else
            {
                return new DataRow[] { };
            }
        }
        
        public DataTable table
        {
            get { return this.view.Table; }
        }

    }

    /*
    using an index on a DataTable:

    1.  this.tbgetTaxes.PrimaryKey = new DataColumn[1] { this.tbgetTaxes.Columns["fld_invoice"] };
        this.tbgetTaxes.Rows.Find("1034567");   this will now use an internal created index for the primary key
            
    2.  DataView v = new DataView(this.tbgetTaxes);  //using a internal created index
        v.Sort = "fld_invoice ASC, cust DESC";
        DataRowView[] res = v.FindRows("1034567");
            OR ? 
        v.RowFilter = "fld_invoice=1034567";
    */
}
