using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ORM.DBFields;

namespace ORM.db_store
{
    public class TableRowColumnsWrap : ITableColumnsWrap
    {
        private TableRow genericTableRow;    
        
        public TableRowColumnsWrap(TableRow gtr)
        {
            this.genericTableRow = gtr;
        }
        
        public IEnumerable<string> getColumns()
        {
            foreach (string f in this.genericTableRow.fields.Keys)
            {
                yield return f;
            }
        }

        public bool isComputed(string columnName)
        {
            return this.genericTableRow.fields[columnName].isComputed;
        }

        public bool isRequired(string columnName)
        {
            return this.genericTableRow.fields[columnName].isRequired;
        }
        public ColumnAttributes columnAttributes(string columnName)
        {
            return this.genericTableRow.db.columns(this.genericTableRow.dbObjectName)[columnName];
        }
        public string fieldClassType(string columnName)
        {
            return this.genericTableRow.fields[columnName].GetType().Name;
        }

        public Type dataTableColumnType(string columnName)
        {
            string type = this.fieldDotNetType(columnName);
            if (type.EndsWith("?")) { type = type.Substring(0, type.Length - 1); }
            if (!type.StartsWith("System.")) { type = "System." + type; }
            return Type.GetType(type);
        }

        public string fieldDotNetType(string columnName)
        {
            return this.genericTableRow.fields[columnName].valueTypeName;
        }

        public string classParams(string columnName, string className)
        {
            return this.classParams(this.genericTableRow.fields[columnName], className);
        }

        private string classParams(GenericField column, string className)
        {
            string res = "";
            switch (className)
            {
                case "FChar":
                case "FVarchar":
                case "FGuid":
                case "FXML":
                    res = string.Format("(\"{0}\", {1}, {2}, false)", column.name, column.isRequired.ToString().ToLower(), ((FChar)column).length);
                    break;  //(string name, bool allowNull, int length)
                case "FText":
                    res = string.Format("(\"{0}\", {1}, false)", column.name, column.isRequired.ToString().ToLower());
                    break;  //(string name, bool allowNull)
                case "FFloat":
                case "FInteger":
                    res = string.Format("(\"{0}\", {1}, false, false)", column.name, column.isRequired.ToString().ToLower());
                    break; //(string name, bool allowNull, bool isIdentity)
                case "FBoolean":
                case "FDatetime":
                case "FTimeSpan":
                case "FByteArray":
                case "FImage":
                case "FVarBinary":
                    res = string.Format("(\"{0}\", {1}, false)", column.name, column.isRequired.ToString().ToLower());
                    break;  //(string name, bool allowNull)
            }
            return res;
        }

    }
}
