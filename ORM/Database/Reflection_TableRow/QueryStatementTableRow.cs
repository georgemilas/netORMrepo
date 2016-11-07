using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using ORM.exceptions;
using ORM.DBFields;
using System.Text.RegularExpressions;
using ORM.db_store;
using EM.DB;

namespace ORM
{
    public class QueryStatementTableRow: TableRow
    {
        private string origSQL;
        public SQLStatement querySQL;
        
        private DataTable _defTable;
        public virtual DataTable defTable
        {
            get { return _defTable; }
            set { _defTable = value; }
        }

        /// <summary>
        /// to alow the inheritance chain, but the logic will not use this
        /// </summary>
        protected QueryStatementTableRow(ORMContext context): base(context) 
        { }

        private QueryStatementTableRow(QueryStatementTableRow other)
            : this(other.context)
        {
            this.origSQL = other.origSQL;
            this.querySQL = other.querySQL;
            this.defTable = other.defTable;
            this.setFields();
            this.dbObjectName = other.dbObjectName;
            this.pk = other.pk;
            this.fk = other.fk;
            this._isReadOnly = true;

            this.init();
            //this.initPresentation();
        }
        
        public QueryStatementTableRow(ORMContext context, string sql)
            : this(context)
        {
            this.origSQL = sql;
            this.querySQL = SQLStatement.getFromSQLQuery(context.db, sql);
            string fields = querySQL.fields;
            //run it once to get the definition of columns
            querySQL.fields = "TOP 1 " + fields;
            this.defTable = context.db.db.getDataTable(querySQL.selectSql());
            querySQL.fields = fields;

            this.setFields();
            this._isReadOnly = true;

            //table name
            Regex frm = new Regex(@"(left\s+join|right\s+join|inner\s+join|left\s+outer\s+join|right\s+outer\s+join|join)\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match frmMatch = frm.Match(querySQL.from);
            int len = querySQL.from.Length - querySQL.from.ToLower().IndexOf("from ") - 5;
            if (frmMatch.Success)
            {
                len = frmMatch.Index - querySQL.from.ToLower().IndexOf("from ") - 5;
            }
            string tn = querySQL.from.Substring(querySQL.from.ToLower().IndexOf("from ")+5, len);
            this.setTableName(TableName.fromDotString(tn.Trim()));

            //inherit support
            this.init();
            //this.initPresentation();
        }

        public void setTableName(TableName tableName)
        {
            this.dbObjectName = tableName;
            this.dbObjectName.context = this.context;
            this.pk = new PKInfo();
            this.pk.table = this.dbObjectName;
            this.fk = new FKInfo();
            this.fk.table = this.dbObjectName;
        }

        protected void setFields()
        {
            foreach (DataColumn c in this.defTable.Columns)
            {
                string ftype = this.context.db.fieldClassType(c);
                this.fields[c.ColumnName] = this.getField(c, ftype);
                this.fields[c.ColumnName].table = this;
            }
        }

        protected void setSQLAttributes(ref SQLStatement stm)  
        {
            if (stm.fields == null) { stm.fields = querySQL.fields; }
            if (stm.from == null) { stm.from = querySQL.from; }
            if (stm.where == null) { stm.where = querySQL.where; }
            if (stm.group == null) { stm.group = querySQL.group; }
            if (stm.having == null) { stm.having = querySQL.having; }
            if (stm.order == null) { stm.order = querySQL.order; }                    
        }
        
        public override TableRow getInstance()
        {
            return new QueryStatementTableRow(this);            
        }


        public virtual Table<T> select<T>() where T : TableRow
        {
            return select<T>(new SQLStatement(this), null);
        }
        /// <summary>
        /// SQLStatement are ignored in the case of stored procedures
        /// </summary>
        public virtual Table<T> select<T>(SQLStatement atr) where T : TableRow
        {
            return select<T>(atr, null);
        }
        /// <summary>
        /// SQLStatement are ignored in the case of stored procedures
        /// </summary>
        public virtual Table<T> select<T>(SQLStatement atr, DBParams param) where T : TableRow
        {
            this.setSQLAttributes(ref atr);
            DataTable tb = db.select(this, atr, param);     //ABSTRACT STORAGE         
            return this.getInstancesFromDataTable<T>(tb);
        }

        /// <summary>
        /// use this in case you have to much data and instances of actual TableRow objects would consume to much memory
        /// </summary>
        public virtual DataTable selectDataTable() { return selectDataTable(new SQLStatement(this), null); }
        public virtual DataTable selectDataTable(SQLStatement atr) { return selectDataTable(atr, null); }
        public virtual DataTable selectDataTable(SQLStatement atr, DBParams param)
        {
            this.setSQLAttributes(ref atr);
            return db.select(this, atr, param);     //ABSTRACT STORAGE         
        }




        protected GenericField getField(DataColumn column, string className) 
        {
            Type tp = null;
            //TODO: optimize this loop with a dict search
            Assembly a  = Assembly.GetExecutingAssembly();
            foreach (Type t in a.GetTypes())
            {
                if (t.Name == className)
                {
                    tp = t;
                    break;
                }
            }
            if (tp == null) throw new ORMException(string.Format("no class with name of {0} was found", className));

            switch (className)
            {
                case "FChar":
                case "FVarchar":
                case "FGuid":
                case "FXML":
                    return (GenericField)Activator.CreateInstance(tp, column.ColumnName, column.AllowDBNull, column.MaxLength);
                case "FText":
                    return (GenericField)Activator.CreateInstance(tp, column.ColumnName, column.AllowDBNull);
                case "FFloat":
                case "FInteger":
                    return (GenericField)Activator.CreateInstance(tp, column.ColumnName, column.AllowDBNull, false, false);
                case "FBoolean":
                case "FDatetime":
                case "FTimeSpan":
                case "FByteArray":
                case "FImage":
                case "FVarBinary":
                    return (GenericField)Activator.CreateInstance(tp, column.ColumnName, column.AllowDBNull, false);
            }

            return null;
            
        }

    }
}
