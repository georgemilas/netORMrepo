using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.db_store
{
    public class ColumnAttributes
    {
        public bool allowNull = true;
        public string dbType;
        public string dotNetType;
        public int maxLength = -1;    //null
        public bool autoIncrement = false;
        public bool hasDefaultConstraint = false;
        public bool isComputed = false;

        public byte? numericPrecision = null;
        public int? numericScale = null;
        public string defaultValue = null;  // int -> ('0') 

        public ColumnAttributes() { }
        public ColumnAttributes(bool allowNull, string dbType, string dotNetType)
        {
            this.allowNull = allowNull;
            this.dbType = dbType;
            this.dotNetType = dotNetType;
        }
        public ColumnAttributes(bool allowNull, string dbType, string dotNetType, int max_length)
        {
            this.allowNull = allowNull;
            this.dbType = dbType;
            this.dotNetType = dotNetType;
            this.maxLength = max_length;
        }
        public ColumnAttributes(bool allowNull, string dbType, string dotNetType,
            int max_length, bool autoIncrement, bool hasDefaultConstraint, bool isComputed)
        {
            this.allowNull = allowNull;
            this.dbType = dbType;
            this.dotNetType = dotNetType;
            this.maxLength = max_length;
            this.autoIncrement = autoIncrement;
            this.hasDefaultConstraint = hasDefaultConstraint;
            this.isComputed = isComputed;
        }
        
        public override string ToString()
        {
            return dbType + (maxLength >= 0 ? "("+maxLength+")" : "") + " " + (allowNull ? "NULL" : "NOT NULL") + " " + (autoIncrement ? "IDENTITY" : "") + " " + (isComputed ? "COMPUTED" : "") + " " + (hasDefaultConstraint ? "DEFAULT" : "");
        }
    }

}
