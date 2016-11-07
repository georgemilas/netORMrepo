using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using System.Data;

namespace ORM.db_store
{    
    public class TableColumnsInfo : OrderedDictionary<string, ColumnAttributes>
    {
        public TableName table;
        
        public TableColumnsInfo(TableName table) { }

        public TableColumnsInfo(DataTable tb, TableName table)
        {
            this.table = table;
            foreach (DataRow r in tb.Rows)
            {
                this[r[1].ToString()] = new ColumnAttributes();
                this[r[1].ToString()].allowNull = r[2].ToString().ToLower().Trim()=="no" ? false : true;
                this[r[1].ToString()].dbType = r[3].ToString();
                this[r[1].ToString()].dotNetType = null;
                this[r[1].ToString()].maxLength = r[4] == System.DBNull.Value ? -1 : (int)r[4];
                this[r[1].ToString()].autoIncrement = (r[5] != System.DBNull.Value && (int)r[5] == 1) ? true : false;
                this[r[1].ToString()].hasDefaultConstraint = (int)r["hasDefaultConstraint"] == 1 ? true : false;
                this[r[1].ToString()].isComputed = r["isComputed"] != System.DBNull.Value ? (int)r["isComputed"] == 1 ? true : false : false;
                this[r[1].ToString()].numericPrecision = r["NumericPrecision"] != System.DBNull.Value ? (byte?)r["NumericPrecision"] : null;
                this[r[1].ToString()].numericScale = r["NumericScale"]!= System.DBNull.Value ? (int?)r["NumericScale"] : null;
                this[r[1].ToString()].defaultValue = r["DefaultValue"].ToString();

                //TODO: make this SQL Server specific:
                if (this[r[1].ToString()].dbType.ToLower() == "timestamp")
                {
                    //in sql server we cannot set an explicit value in a timestamp column
                    this[r[1].ToString()].isComputed = true;  
                }

            }
        }
        
        public TableColumnsInfo(DataColumnCollection tb, TableName table)
        {
            this.table = table;
            foreach (DataColumn c in tb)
            {
                this[c.ColumnName] = new ColumnAttributes();
                this[c.ColumnName].allowNull = c.AllowDBNull;
                this[c.ColumnName].dbType = null;
                this[c.ColumnName].dotNetType = c.DataType.ToString().Replace("System.", "");
                if (this[c.ColumnName].dotNetType.ToLower() != "string")
                {
                    if (!this[c.ColumnName].dotNetType.EndsWith("[]"))  //not array
                    {
                        this[c.ColumnName].dotNetType += "?";  //make value types nullable
                    }
                }
                this[c.ColumnName].maxLength = c.MaxLength;
                this[c.ColumnName].autoIncrement = c.AutoIncrement;
                this[c.ColumnName].hasDefaultConstraint = false;
                this[c.ColumnName].isComputed = false;
                this[c.ColumnName].numericPrecision = null;
                this[c.ColumnName].numericScale = null;
                this[c.ColumnName].defaultValue = null;  //not c.DefaultValue because it looks like that is not based on db default constraint
            }
        }
        
    }
}
