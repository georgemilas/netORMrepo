using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using EM.DB;
using EM.Collections;
using ORM.exceptions;
using System.Data.Common;
using ORM.Database;

namespace ORM
{
    

    /// <summary>
    /// A somewat generic database backend
    /// Create a SpecificDatabse(Database) class tuned for your own database type
    /// See http://troels.arvin.dk/db/rdbms/ for rdbms differences     
    ///     
    ///     When overiding and building a diferent storage mechanism then an actual database,
    ///     a middle layer perhaps, the you should rewrite most of it, but for actual work at least        
    ///          - limitAndOffset(PKInfo pk, SQLAttributes atr)   
    ///          - save(TableRowList tb)
    ///          - insert, update, delete, select (TableRow tb, SQLAttributes atr, DBParams p)   
    ///          - maybe abstract string getLastInsertID(TableRow tb);
    ///
    ///     and for generation, also:
    ///         abstract tables { get; }
    ///         abstract views { get; }
    ///         //abstract tb_autofield();
    ///         abstract pk(TableName table);
    ///         abstract fk(TableName table);
    ///         abstract string DBName { get; }
    ///         abstract fieldClassType(string column, TableColumnsInfo tci);
    ///         abstract fieldDotNetType(string column, TableColumnsInfo tci);
    ///         
    /// </summary>
    public abstract class GenericDatabase
    {
        public BaseDBWorker db;

        public GenericDatabase() { }
        public GenericDatabase(BaseDBWorker db)
        {
            this.db = db;
        }

        
        #region abstract
        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////  ABSTRACT MEMBERS
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract EList<TableName> tables { get; }
        public abstract EList<TableName> views { get; }
        public abstract EList<TableName> storedProcedures { get; }

        public abstract string tb_autofield();
        public abstract ILastInsertIDProvider getLastInsertID(TableRow tb);
        //sql server: "SELECT SCOPE_IDENTITY()";
        //sql server (??? insert triggers that insert themselfs somewhere): "SELECT @@IDENTITY"
        //sql server: select IDENT_CURRENT('tbl_cust')  -> return last inserted id in this table from any scope any connection
        //postgres: select curval(tbl_cust_id_seq)
        //mysql: select LAST_INSERT_ID()

        public abstract PKInfo pk(TableName table);
        public abstract FKInfo fk(TableName table);
        public abstract string DBName { get; }
        public abstract string fieldClassType(string column, TableColumnsInfo tci);
        public abstract string fieldDotNetType(string dbType);
        public abstract object getDbTypeDummyValue(string dbType);
        public abstract StoredProcDef storedProcedure(TableName procName);

        #endregion abstract

        
        protected string fixName(string name)
        {
            return ORMContext.fixName(name);    // name.Replace(" ", "");
        }

        public string getClassName(TableName table, EList<TableName> allNeddedTables)
        {
            //we now use schema as part of namespace so we don't need this anymore
            /*
            int c = 0;
            allNeddedTables.ForEach(delegate(TableName t) { if (t.table == table.table) c += 1; });
            if (c > 1) { return fixName(table.schema + "_" + table.table); }
             */
            return fixName(table.table);
        }

        
        #region virtual        
        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////  VIRTUAL MEMBERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual string escape(string identifier)
        {
            return string.Format("\"{0}\"", identifier);
        }
        
        public virtual string selectSql(SQLAttributes atr)
        {
            return atr.selectSql();
        }

        public virtual SQLAttributes limitAndOffset(PKInfo pk, SQLAttributes atr)
        {
            if (atr.limit > 0)
                atr.where += " LIMIT " + atr.limit.ToString();
            if (atr.offset > 0)
                atr.where += " OFFSET " + atr.offset.ToString();
            return atr;
        }        

        public virtual TableColumnsInfo columns(TableName table)
        {
            SQLAttributes atr = new SQLAttributes(this);
            atr.fields = "*";
            atr.from = this.escape(table.schema) + "." + this.escape(table.table);
            atr.limit = 1;
            DataTable tb = this.db.getDataTable(atr.selectSql());
            return new TableColumnsInfo(tb.Columns, table);
        }

        
        public virtual string fieldDotNetType(string column, TableColumnsInfo tci)
        {
            try
            {
                return this.fieldDotNetType(tci[column].dbType);
            }
            catch   //(Exception e)
            {
                Console.WriteLine(tci[column].dbType);
                throw new Exception(tci[column].dbType + " was not defined");
            }
        }

        public virtual string fieldClassType(DataColumn c)
        {
            string data = @"{'System.String':'FVarchar', 
                             'System.Guid':'FGuid',
                             'System.Char':'FChar', 
                             'System.Int16':'FInteger', 
                             'System.Int32':'FInteger', 
                             'System.Int64':'FInteger', 
                             'System.Byte':'FInteger', 
                             'System.Double':'FFloat', 
                             'System.Decimal':'FFloat', 
                             'System.DateTime':'FDatetime',
                             'System.Byte[]':'FByteArray', 
                             'System.Boolean':'FBoolean'}";

            EDictionary<string, string> d = EDictionary<string, string>.fromStrStr(data);
            return d[c.DataType.ToString()];
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /////      MAIN Storage  DATA ENGINE
        /////      
        public virtual DataTable selectStoredProcedure(TableName spname)
        {
            return this.selectStoredProcedure(spname, new DBParams());
        }
        public virtual DataTable selectStoredProcedure(TableName spname, DBParams p)
        {
            return this.db.getDataTable(spname.sqlFromName, p, CommandType.StoredProcedure);
        }
        public virtual bool executeStoredProcedure(TableName spname, DBParams p)
        {
            return this.db.executeQuery(spname.sqlFromName, p, CommandType.StoredProcedure);
        }
        
        public virtual DataTable select(TableRow tb, SQLAttributes atr, DBParams p)
        {
            string sql = atr.selectSql();
            return this.db.getDataTable(sql, p);
        }

        public virtual object insert(TableRow tb, SQLAttributes atr, DBParams p)
        {
            string sql = atr.insertSql();
            //Console.WriteLine(sql);
            //Console.WriteLine(p.ToString());
            if (tb.pk.Count == 1 && tb.fields[tb.pk[0]].isIdentity)
            {
                return this.db.executeInsertSql(sql, p, this.getLastInsertID(tb));                
            }
            else
            {
                return this.db.executeQuery(sql, p);                
            }

        }

        public virtual bool update(TableRow tb, SQLAttributes atr, DBParams p)
        {
            string sql = atr.updateSql();
            //Console.WriteLine(sql);
            return this.db.executeQuery(sql, p);
        }

        public virtual bool delete(TableRow tb, SQLAttributes atr, DBParams p)
        {
            string sql = atr.deleteSql();
            //Console.WriteLine(sql);
            return this.db.executeQuery(sql, p);
        }
        
        #endregion virtual

    }
}


