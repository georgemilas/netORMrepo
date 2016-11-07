using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ORM.DBFields;
using ORM.db_store.persitence;
//using ORM.generator;

namespace ORM.db_store
{
    public class TableColumnsInfoColumnsWrap : ITableColumnsWrap
    {
        private TableColumnsInfo tableColumnsInfo;
        
        private GenericDatabase gdb;


        //public TableColumnsInfoColumnsWrap(Generator g, TableColumnsInfo tci) : this(g.db, tci) { }
        public TableColumnsInfoColumnsWrap(GenericDatabase gdb, TableColumnsInfo tci)
        {
            this.tableColumnsInfo = tci;
            this.gdb = gdb;
        }

        public IEnumerable<string> getColumns()
        {
            foreach (string c in this.tableColumnsInfo.Keys)
            {
                yield return c;
            }
        }

        public bool isComputed(string columnName)
        {
            return (bool)this.tableColumnsInfo[columnName].isComputed;
        }

        public bool isRequired(string columnName)
        {
            return !this.tableColumnsInfo[columnName].allowNull;
        }

        public ColumnAttributes columnAttributes(string columnName)
        {
            return this.tableColumnsInfo[columnName]; 
        }

        public string fieldClassType(string columnName)
        {
            return this.gdb.fieldClassType(columnName, tableColumnsInfo);
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
            return tableColumnsInfo.table.customFieldPropertyType.get(columnName, this.gdb.fieldDotNetType(columnName, this.tableColumnsInfo));
            //return this.gdb.fieldDotNetType(columnName, this.tableColumnsInfo);
        }

        public string classParams(string columnName, string className)
        {
            return this.classParams(columnName, className, this.tableColumnsInfo);
        }

        private string classParams(string column, string className, TableColumnsInfo tci)
        {
            string res = "";
            switch (className)
            {
                case "FChar":
                case "FVarchar":
                case "FGuid":
                case "FXML":
                    res = string.Format("(\"{0}\", {1}, {2}, {3})", column, tci[column].allowNull.ToString().ToLower(), tci[column].maxLength, tci[column].hasDefaultConstraint.ToString().ToLower());
                    break;  //(string name, bool allowNull, int length, bool hasDefaultConstraint)
                case "FText":
                    res = string.Format("(\"{0}\", {1}, {2})", column, tci[column].allowNull.ToString().ToLower(), tci[column].hasDefaultConstraint.ToString().ToLower());
                    break;  //(string name, bool allowNull)
                case "FFloat":
                case "FInteger":
                    res = string.Format("(\"{0}\", {1}, {2}, {3})", column, tci[column].allowNull.ToString().ToLower(), tci[column].hasDefaultConstraint.ToString().ToLower(), tci[column].autoIncrement.ToString().ToLower());
                    break; //(string name, bool allowNull, bool hasDefaultConstraint, bool isIdentity)
                case "FBoolean":
                case "FDatetime":
                case "FTimeSpan":
                case "FByteArray":
                case "FImage":
                case "FVarBinary":
                    res = string.Format("(\"{0}\", {1}, {2})", column, tci[column].allowNull.ToString().ToLower(), tci[column].hasDefaultConstraint.ToString().ToLower());
                    break;  //(string name, bool allowNull, bool hasDefaultConstraint)
            }
            return res;
        }

    }
}
