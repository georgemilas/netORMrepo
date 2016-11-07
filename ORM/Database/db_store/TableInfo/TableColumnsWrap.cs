using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ORM.DBFields;
using ORM.db_store;
using ORM.db_store.persitence;

namespace ORM.db_store
{
    /// <summary>
    /// - a generic (polymorfic) table wrap 
    /// - builds real wrap based on the constuctors parameters 
    /// </summary>
    public class TableColumnsWrap : ITableColumnsWrap
    {
        ITableColumnsWrap realWrap;
        public TableColumnsWrap(TableRow gtr)
        {
            realWrap = new TableRowColumnsWrap(gtr);
        }
        
        public TableColumnsWrap(GenericDatabase gdb, DataTable table)
        {
            realWrap = new DataTableColumnsWrap(gdb, table);
        }

        public TableColumnsWrap(GenericDatabase gdb, TableColumnsInfo tci)
        {
            realWrap = new TableColumnsInfoColumnsWrap(gdb, tci);
        }

        public IEnumerable<string> getColumns()
        {
            return realWrap.getColumns();
        }

        public bool isComputed(string columnName)
        {
            return realWrap.isComputed(columnName);
        }

        public bool isRequired(string columnName)
        {
            return realWrap.isRequired(columnName);
        }
        public ColumnAttributes columnAttributes(string columnName)
        {
            return this.realWrap.columnAttributes(columnName);
        }
        public string fieldClassType(string columnName)
        {
            return realWrap.fieldClassType(columnName);
        }

        public Type dataTableColumnType(string columnName)
        {
            return realWrap.dataTableColumnType(columnName);            
        }

        public string fieldDotNetType(string columnName)
        {
            var tp =  realWrap.fieldDotNetType(columnName);
            if (this.isRequired(columnName))
            {
                tp = tp.Replace("?", "");
            }
            return tp;
        }

        public string classParams(string columnName, string className)
        {
            return realWrap.classParams(columnName, className);
        }

    }
}
