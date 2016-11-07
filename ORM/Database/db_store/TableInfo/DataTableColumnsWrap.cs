using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ORM.DBFields;
using ORM.db_store.persitence;
//using ORM.generator;

namespace ORM.db_store
{
    public class DataTableColumnsWrap : ITableColumnsWrap
    {
        private DataTable table;    
        
        private GenericDatabase gdb;

        //public DataTableColumnsWrap(Generator g, DataTable table) : this(g.db, table) { }
        public DataTableColumnsWrap(GenericDatabase gdb, DataTable table)
        {
            this.table = table;
            this.gdb = gdb;
        }

        public IEnumerable<string> getColumns()
        {
            foreach (DataColumn c in this.table.Columns)
            {
                yield return c.ColumnName;
            }
        }

        public bool isComputed(string columnName)
        {
            return false;
        }

        public bool isRequired(string columnName)
        {
            return false;
        }
        public ColumnAttributes columnAttributes(string columnName)
        {
            return new TableColumnsInfo(this.table.Columns, new TableName("", "", table.TableName))[columnName];
        }
        public string fieldClassType(string columnName)
        {
            return this.gdb.fieldClassType(this.table.Columns[columnName]);
        }

        public Type dataTableColumnType(string columnName)
        {
            return this.table.Columns[columnName].DataType;
        }

        public string fieldDotNetType(string columnName)
        {
            string ft = this.table.Columns[columnName].DataType.Name;
            if (!ft.ToLower().Contains("string") && !ft.ToLower().Contains("[]"))
            {
                ft = ft + "?";  //nullable type
            }
            return ft;
        }

        public string classParams(string columnName, string className)
        {
            return this.classParams(this.table.Columns[columnName], className);
        }

        private string classParams(DataColumn column, string className)
        {
            string res = "";
            switch (className)
            {
                case "FChar":
                case "FVarchar":
                case "FGuid":
                case "FXML":
                    res = string.Format("(\"{0}\", {1}, {2}, false)", column.ColumnName, column.AllowDBNull.ToString().ToLower(), (int)column.MaxLength);
                    break;  //(string name, bool allowNull, int length)
                case "FText":
                    res = string.Format("(\"{0}\", {1}, false)", column.ColumnName, column.AllowDBNull.ToString().ToLower());
                    break;  //(string name, bool allowNull)
                case "FFloat":
                case "FInteger":
                    res = string.Format("(\"{0}\", {1}, false, false)", column.ColumnName, column.AllowDBNull.ToString().ToLower());
                    break; //(string name, bool allowNull, bool isIdentity)
                case "FBoolean":
                case "FDatetime":
                case "FTimeSpan":
                case "FByteArray":
                case "FImage":
                case "FVarBinary":
                    res = string.Format("(\"{0}\", {1}, false)", column.ColumnName, column.AllowDBNull.ToString().ToLower());
                    break;  //(string name, bool allowNull)
            }
            return res;
        }

    }
}
