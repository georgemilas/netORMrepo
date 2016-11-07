using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using EM.Collections;
using EM.Logging;

using EM.Cache;
using System.Web.Caching;


namespace EM.DB
{
    /// <summary>
    /// - Generic Sql Worker Class 
    /// - may not use a contructor but the factory method (GetWorker) below, so that it returns the corect class instance based of config 
    ///   parameters (ex. to use Worker or NetworkAwareWorker?)
    /// </summary> 
    public abstract class BaseDBWorker : IDisposable, IDBWorker
    {

        protected EDictionary<string, string> config;   //alow configuration via this config file or if not given use default XML app config
        protected DbTransaction _currentTransaction;

        private string _CONN_STR; // database connection string
        public string CONN_STR
        {
            get { return _CONN_STR; }
            set { _CONN_STR = value; }
        }

        public string uniqueQueryID;     //for example current invoice number
        protected string _contextCommand;    //for example to add a user to all the commands to come (set context_info -1428)
        private ICacheProvider<string> _cache;     //- if you know that your select statements always returns the same DataTable

        public ICacheProvider<string> cache
        {
            get { return _cache; }
            set { _cache = value; }
        }
        //     you can pass a cahe object (ex. EUtil.DB.Cache.DictCache)
        //     to have the DataTable for the select statement be cached for subseqvent runs
        //- only getDataTable, getDataSet are using this
        //- the keys in the cache will be "CONN_STR + sqlStatement" so in some cases probably 
        //     you can reuse the same cache acros multiple instances of SqlServerDBWorker



        private bool _raise = true;
        public bool raise
        {
            get { return _raise; }
            set { _raise = value; }
        }

        private ILogger _logger;
        private bool _sqlTrace;
        private ILogLevel saveLoggerLevel;


        private int _commandTimeOut = 0;      //no timeout by default
        public int commandTimeOut
        {
            get { return _commandTimeOut; }
            set { _commandTimeOut = value; }
        }
        public int maxNumberOfTries = 4;    // if a query fails, incrise timeout and try again maxNumberOfTries times

        public abstract DbDataAdapter getDataAdapter();
        public abstract DbCommand getDataCommand(string sqlStr);
        public abstract DbConnection connection { get; }
        public abstract DBParams getNewDBParams();

        /// <summary>
        /// must start a connection
        /// </summary>
        public abstract void init();
        public abstract void Dispose();

        protected virtual string getCacheKey(string sql, IDBParams param)
        {
            StringBuilder sb = new StringBuilder(this.CONN_STR);
            sb.Append(sql);
            if (param != null)
            {
                foreach (IDBParam p in param)
                {
                    sb.Append(p.param.ParameterName + "=" + p.param.Value.ToString());
                }
            }
            return sb.ToString();
        }

        protected virtual void setToCache(string key, Object val, DbCommand command)
        {
            if (this.cache != null)
            {
                ICacheDependency dep = null; //dependency is only used for sql server
                CacheValue<string> cv = new CacheValue<string>(val, this.cache.time_out_period, dep);
                this.cache.set(key, cv);
            }
        }

        public BaseDBWorker()
        {
            this.CONN_STR = null;
            this.cache = null;
            this.raise = true;
            this.contextCommand = null;
            this.uniqueQueryID = "-1";
        }

        public BaseDBWorker(string connStr)
        {
            this.CONN_STR = connStr;
            this.cache = null;
            this.raise = true;
            this.contextCommand = null;
            this.uniqueQueryID = "-1";
            this.init();
        }

        public BaseDBWorker(EDictionary<string, string> config)
        {
            this.config = config;

            if (config == null)
            {
                this.CONN_STR = ConfigurationManager.AppSettings.Get("CONN_STR");
            }
            else
            {
                this.CONN_STR = config["CONN_STR"];
            }
            this.cache = null;
            this.raise = true;
            this.contextCommand = null;
            //this.init();
        }

        public ILogger logger
        {
            get { return this._logger; }
            set { this._logger = value; }
        }


        public virtual void writeLog(string err_id, string msg, Exception e, string sql, IDBParams dbparams)
        {
            if (this.logger != null)
            {
                string details = sql;
                if (dbparams != null && dbparams.Count > 0)
                {
                    StringBuilder sb = new StringBuilder("SQL: " + sql);
                    sb.Append(StringUtil.CRLF + "Params:");
                    foreach (IDBParam p in dbparams)
                    {
                        sb.Append(StringUtil.CRLF + p.param.ParameterName + " = " + (p.param.Value == null ? "NULL" : p.param.Value.ToString()));
                    }
                    details = sb.ToString();
                }

                this.logger.write(err_id, this._logger.level, msg, e, details);
            }
        }

        /// <summary>
        /// - if current transaction is set then all operations will use it (getDataTable, executeQuery, executeScalar etc...)
        /// - if an error occure during the db opperation, the transaction is rolled back (raises or not based on raiseErrors)
        /// </summary>
        public virtual DbTransaction currentTransaction
        {
            get { return this._currentTransaction; }
            set { this._currentTransaction = value; }
        }
        public virtual DbTransaction startTransaction() 
        {
            DbTransaction trans = this.connection.BeginTransaction();
            return trans;
        }
        public virtual DbTransaction startTransaction(IsolationLevel level)
        {
            DbTransaction trans = this.connection.BeginTransaction(level);
            return trans;
        }

        public virtual void restoreConnection()
        {
            if (this.connection.State == ConnectionState.Broken || this.connection.State == ConnectionState.Closed)
            {
                this.init();
            }
        }

        /// <summary>
        ///    -for example to add the user gmilas to all the commands to come you will do 
        ///    - db.contextCommand = "set context_info 'gmilas'"
        ///       - this will execute the command and if later the connection needs to be reoppened 
        ///         it will reissue the contextCommand automaticaly as long as the field is not null
        ///    - to remove the contex from db you might need to issue a complementary command and then set the field to null
        ///    - only assigning null or "" to the field will not run anything in the database but just make the field null 
        /// </summary>
        public string contextCommand
        {
            get { return this._contextCommand; }
            set
            {
                if (value != null && value.Trim() != "")
                {
                    this.executeQuery(value, true);
                    this._contextCommand = value;
                }
                else
                {
                    this._contextCommand = null;
                }
            }
        }


        public virtual DataTable getSchemaTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {                
                return _getSchemaTable(sqlQ, dbparams, cmdType);
            }
        }
        protected DataTable _getSchemaTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            if (this.cache != null)
            {
                string key = this.getCacheKey(sqlQ, dbparams);
                if (this.cache.has_key(key))
                {
                    return (DataTable)this.cache.get(key);
                }
            }

            DataTable dt = new DataTable();
            DbDataAdapter myAdapter = this.getDataAdapter();

            DbCommand myCommand = this.getDataCommand(sqlQ);
            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                DbDataReader rd;
                try
                {
                    rd = myCommand.ExecuteReader(CommandBehavior.SchemaOnly);
                }
                catch (DbException)
                {
                    //The schema-only execution may fail if temp tables are generated in the stored proc
                    //if so, try a full execution of the stored procedure
                    rd = myCommand.ExecuteReader();
                }
                dt = rd.GetSchemaTable();
                rd.Close();
                rd.Dispose();

                if (this.cache != null)
                {
                    //this.cache.set(this.CONN_STR + sqlQ, dt, myCommand);
                    string key = this.getCacheKey(sqlQ, dbparams);
                    setToCache(key, dt, myCommand);
                }
                else
                {
                    myCommand.Dispose();
                }

            }

            catch (DbException e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getSchemaTable error", e, sqlQ, dbparams);
                dt.Clear();
                myCommand.Dispose();
                throw;

            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getSchemaTable error", e, sqlQ, dbparams);
                dt.Clear();
                myCommand.Dispose();
                throw;
            }

            return dt;
        }

        //############################################################################################################
        // DataTable
        //############################################################################################################
        public virtual DataTable getDataTable(string tableName, string sqlQ) { return getDataTable(tableName, sqlQ, null); }
        public virtual DataTable getDataTable(string tableName, string sqlQ, IDBParams dbparams) { return getDataTable(tableName, sqlQ, dbparams, CommandType.Text); }
        public virtual DataTable getDataTable(string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            DataTable tb = this.getDataTable(sqlQ, dbparams, cmdType);
            if (tableName != null)
            {
                tb.TableName = tableName;
            }
            return tb;
        }
        public virtual DataTable getDataTable(string sqlQ) { return getDataTable(sqlQ, null, CommandType.Text); }
        public virtual DataTable getDataTable(string sqlQ, IDBParams dbparams) { return getDataTable(sqlQ, dbparams, CommandType.Text); }
        public virtual DataTable getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            //need to lock basicaly because if I use this in ASP.NET with a cached SqlServerDBWorker instance 
            //(for cached connection should be lock (this.sqlConn))    
            //and otherwise if 2 pages run in the same time, when it does myAdapter.Fill(dt) it will throw Command already has a DataReader associated
            lock (this)
            {
                return _getDataTable(sqlQ, dbparams, cmdType);
            }
        }
        protected DataTable _getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            if (this.cache != null)
            {
                string key = this.getCacheKey(sqlQ, dbparams);
                if (this.cache.has_key(key))
                {
                    return (DataTable)this.cache.get(key);
                }
            }

            DataTable dt = new DataTable();
            DbDataAdapter myAdapter = this.getDataAdapter();

            DbCommand myCommand = this.getDataCommand(sqlQ);
            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                myAdapter.SelectCommand = myCommand;
                myAdapter.Fill(dt);
                //if (numRowsAffected == 0 && myCommand.Connection is SqlConnection) 
                //{
                //    SqlConnection.ClearPool((SqlConnection)myCommand.Connection);
                //}

                if (this.cache != null)
                {
                    //this.cache.set(this.CONN_STR + sqlQ, dt, myCommand);
                    string key = this.getCacheKey(sqlQ, dbparams);
                    setToCache(key, dt, myCommand);
                }
                else
                {
                    myCommand.Dispose();
                    myAdapter.Dispose();
                }

            }
            catch (DbException e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getDataTable error", e, sqlQ, dbparams);
                dt.Clear();
                myCommand.Dispose();
                myAdapter.Dispose();
                if (this.raise) { throw; }

            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getDataTable error", e, sqlQ, dbparams);
                dt.Clear();
                myCommand.Dispose();
                myAdapter.Dispose();
                if (this.raise) { throw; }
            }

            return dt;
        }
        //------------------------------------------
        public static DataTable getDataTable(DataRow dr) { return getDataTable(new DataRow[] { dr }); }
        public static DataTable getDataTable(DataRow[] dr) { return getDataTable(dr, null, null); }
        public static DataTable getDataTable(DataRow[] dr, DataTable dt) { return getDataTable(dr, dt, null); }
        public static DataTable getDataTable(DataRow[] dr, DataTable dt, string tableName)
        {
            if (dt == null)
            {
                if (dr.Length > 0 && dr[0] != null)
                {
                    dt = dr[0].Table;
                }
                else
                {
                    throw new ArgumentException(String.Format("either DataRow[] array was empty or a DataTable was not supplied (table={0})", tableName));
                }
            }
            DataTable t = new DataTable();
            foreach (DataColumn c in dt.Columns)
            { t.Columns.Add(c.ColumnName, c.DataType); }

            foreach (DataRow r in dr)
            { t.Rows.Add(r.ItemArray); }
            if (tableName == null)
            {
                t.TableName = dt.TableName;
            }
            else
            {
                t.TableName = tableName;
            }
            return t;
        }

        //############################################################################################################
        // Display Table
        //############################################################################################################
        /// <summary>
        ///     - display table structure and data (values and types) on the console
        /// </summary>
        public static void displayTable(DataTable t) { displayTable(t, false, null); }
        /// <summary>
        ///     - use schemaFolderPath to write tableName.xsd file or null otherwise
        ///     - use showSchemaOnly to display only table structure without the data otherwise the data will also be displaied
        /// </summary>
        public static void displayTable(DataTable t, bool showSchemaOnly, string schemaFolderPath)
        {
            Console.WriteLine("Table Name: " + t.TableName);

            if (schemaFolderPath != null)
            {
                string path = schemaFolderPath.EndsWith("\\") ? schemaFolderPath : schemaFolderPath + "\\";
                t.WriteXmlSchema(path + t.TableName + ".xsd");
            }

            if (showSchemaOnly)
            {
                Console.WriteLine("Table Schema:");

                foreach (DataColumn c in t.Columns)
                {
                    Console.WriteLine(c.ColumnName + " -> " + c.DataType.ToString());
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Table Data:");

                foreach (DataRow r in t.Rows)
                {
                    foreach (DataColumn c in t.Columns)
                    {
                        Console.WriteLine(c.ColumnName + " = " + r[c].ToString() + " -> " + c.DataType.ToString());
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }


        //############################################################################################################
        // DataSet
        //############################################################################################################
        public virtual DataSet getDataSet(string sqlQ) { return getDataSet(sqlQ, null); }
        public virtual DataSet getDataSet(string sqlQ, IDBParams dbparams) { return this.getDataSet(sqlQ, dbparams, CommandType.Text); }
        public virtual DataSet getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                return _getDataSet(sqlQ, dbparams, cmdType);
            }
        }
        protected DataSet _getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            if (this.cache != null)
            {
                string key = this.getCacheKey(sqlQ, dbparams);
                if (this.cache.has_key(key))
                {
                    return (DataSet)this.cache.get(key);
                }
            }

            DataSet ds = new DataSet();
            //SqlDataAdapter myAdapter = new SqlDataAdapter();
            DbDataAdapter myAdapter = this.getDataAdapter();
            DbCommand myCommand = this.getDataCommand(sqlQ);
            //SqlCommand myCommand = new SqlCommand(sqlQ, this.sqlConn);

            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                myAdapter.SelectCommand = myCommand;
                myAdapter.Fill(ds);

                if (this.cache != null)
                {
                    //this.cache.set(this.CONN_STR + sqlQ, ds, myCommand);
                    string key = this.getCacheKey(sqlQ, dbparams);
                    setToCache(key, ds, myCommand);
                }
                else
                {
                    myCommand.Dispose();
                }

            }
            catch (DbException e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getDataSet error", e, sqlQ, dbparams);
                ds.Clear();
                myCommand.Dispose();
                if (this.raise) { throw; }

            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "getDataSet error", e, sqlQ, dbparams);
                ds.Clear();
                myCommand.Dispose();
                if (this.raise) { throw; }
            }
            return ds;
        }

        //--------------------------------------------------------------------------------------------------------
        public virtual DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ) { return addToDataSet(ref ds, tableName, sqlQ, null); }
        public virtual DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams) { return addToDataSet(ref ds, tableName, sqlQ, dbparams, CommandType.Text); }
        public virtual DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                return _addToDataSet(ref ds, tableName, sqlQ, dbparams, cmdType);
            }
        }
        protected DataSet _addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            //SqlDataAdapter myAdapter = new SqlDataAdapter();
            DbDataAdapter myAdapter = this.getDataAdapter();
            DbCommand myCommand = this.getDataCommand(sqlQ);
            //SqlCommand myCommand = new SqlCommand(sqlQ, this.sqlConn);

            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                myAdapter.SelectCommand = myCommand;
                myAdapter.Fill(ds, tableName);
            }
            catch (DbException e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "addToDataSet error", e, sqlQ, dbparams);
                //ds.Clear();
                if (this.raise) { throw; }
            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog(this.uniqueQueryID, "addToDataSet error", e, sqlQ, dbparams);
                //ds.Clear();
                if (this.raise) { throw; }
            }
            finally
            {
                myCommand.Dispose();
            }

            return ds;
        }
        //############################################################################################################
        // EXECUTE Query
        //############################################################################################################
        /// <summary>
        /// executes an SQL Statement (logges error and raises based on this.raise or parametter raise)
        /// </summary>
        /// <returns>true if successfuly completed</returns>
        public virtual bool executeQuery(string sqlQ) { return executeQuery(sqlQ, this.raise); }
        public virtual bool executeQuery(string sqlQ, IDBParams dbparams) { return executeQuery(sqlQ, this.raise, dbparams); }
        public virtual bool executeQuery(string sqlQ, bool raise) { return executeQuery(sqlQ, raise, null); }
        public virtual bool executeQuery(string sqlQ, bool raise, IDBParams dbparams) { return executeQuery(sqlQ, raise, dbparams, CommandType.Text); }
        public virtual bool executeQuery(string sqlQ, IDBParams dbparams, CommandType cmdType) { return this.executeQuery(sqlQ, this.raise, dbparams, cmdType); }
        public virtual bool executeQuery(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                return _executeQuery(sqlQ, raise, dbparams, cmdType);
            }
        }
        protected bool _executeQuery(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            //SqlCommand myCommand = new SqlCommand(sqlQ, this.sqlConn);
            DbCommand myCommand = this.getDataCommand(sqlQ);

            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }
                myCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog("0", "executeQuery error", e, sqlQ, dbparams);
                if (raise) { throw; }
                return false;
            }
            finally
            {
                myCommand.Dispose();
            }
            return true;
        }


        protected virtual void doSqlTrace(string sqlQ, IDBParams dbparams)
        {
            try
            {
                if (this.logger != null && this.logger.level.level == Level.TRACE)
                {
                    this.logger.trace("SQL_TRACE", StringUtil.CRLF + "------------------------------------------------------>" + StringUtil.CRLF);
                    this.logger.trace("SQL_TRACE", this.connection.ConnectionString + StringUtil.CRLF);
                    this.logger.trace("SQL_TRACE", sqlQ + StringUtil.CRLF);
                    if (dbparams != null)
                    {
                        foreach (IDBParam p in dbparams)
                        {
                            string val = p.param.Value != null ? p.param.Value.ToString() : "NULL";
                            this.logger.trace("SQL_TRACE", "p: " + p.param.ParameterName + " -> " + val + StringUtil.CRLF);
                        }
                    }
                    this.logger.trace("SQL_TRACE", StringUtil.CRLF + "<------------------------------------------------------" + StringUtil.CRLF);
                }
            }
            catch { }
        }

        /// <summary>
        /// executes an SQL Statement and return an object (you will cast to actual type) from the first row/ first column
        ///     - logges error and raises based on this.raise or parametter raise
        /// </summary>
        public virtual object executeScalar(string sqlQ) { return executeScalar(sqlQ, this.raise); }
        public virtual object executeScalar(string sqlQ, IDBParams dbparams) { return executeScalar(sqlQ, this.raise, dbparams); }
        public virtual object executeScalar(string sqlQ, bool raise) { return executeScalar(sqlQ, raise, null); }
        public virtual object executeScalar(string sqlQ, bool raise, IDBParams dbparams) { return executeScalar(sqlQ, raise, dbparams, CommandType.Text); }
        public virtual object executeScalar(string sqlQ, IDBParams dbparams, CommandType cmdType) { return this.executeScalar(sqlQ, this.raise, dbparams, cmdType); }
        public virtual object executeScalar(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                return _executeScalar(sqlQ, raise, dbparams, cmdType);
            }
        }
        protected object _executeScalar(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            object res = null;
            //SqlCommand myCommand = new SqlCommand(sqlQ, this.sqlConn);
            DbCommand myCommand = this.getDataCommand(sqlQ);

            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }
                res = myCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Rollback();
                }
                writeLog("0", "executeScalar error", e, sqlQ, dbparams);
                if (raise) { throw; }
            }
            finally
            {
                myCommand.Dispose();
            }
            return res;
        }

        /// <summary>
        /// executes an Insert SQL Statement and return the id of the row inserted
        ///     - logges error and raises based on this.raise or parametter raise
        ///     - delegate Sql_GetLastInsertId must not be null:
        ///         - so executeInsertSql(string sqlQ) and executeInsertSql(string sqlQ, IDBParams dbparams) will throw
        ///           null pointer exception
        ///         - therefor if a class subclasses from this it should override executeInsertSql(string sqlQ) and
        ///           executeInsertSql(string sqlQ, IDBParams dbparams) and call a base method with a correct delegate 
        /// </summary>
        public virtual int executeInsertSql(string sqlQ) { return executeInsertSql(sqlQ, this.raise, null); }
        public virtual int executeInsertSql(string sqlQ, IDBParams dbparams) { return executeInsertSql(sqlQ, this.raise, dbparams, null); }
        public virtual int executeInsertSql(string sqlQ, ILastInsertIDProvider IdSql) { return executeInsertSql(sqlQ, this.raise, IdSql); }
        public virtual int executeInsertSql(string sqlQ, IDBParams dbparams, ILastInsertIDProvider IdSql) { return executeInsertSql(sqlQ, this.raise, dbparams, IdSql); }
        public virtual int executeInsertSql(string sqlQ, bool raise, ILastInsertIDProvider IdSql) { return executeInsertSql(sqlQ, raise, null, IdSql); }
        public virtual int executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, ILastInsertIDProvider IdSql) { return executeInsertSql(sqlQ, raise, dbparams, CommandType.Text, IdSql); }
        public virtual int executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType, ILastInsertIDProvider IdSql)
        {
            lock (this)
            {
                return _executeInsertSql(sqlQ, raise, dbparams, cmdType, IdSql);
            }
        }
        protected int _executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType, ILastInsertIDProvider IdSql)
        {
            int identity = -1;
            DbTransaction trans = null;
            if (currentTransaction == null)
            {
                trans = this.startTransaction();
            }
            else
            {
                trans = this.currentTransaction;
            }

            //hopefully dot-comma ; is supported by all db engines because this way SQLServer SCOPE_IDENTITY() works
            //otherwise we would need to use IDENT_CURRENT('table_name') or @@IDENTITY
            if (IdSql != null)
            {
                sqlQ += ";" + IdSql.lastInsertID;
            }
            DbCommand myCommand = this.getDataCommand(sqlQ);

            try
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                myCommand.Transaction = trans;
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                if (IdSql != null)
                {
                    object oi = myCommand.ExecuteScalar();
                    identity = int.Parse(oi.ToString());
                }
                else
                {
                    int rowsAffected = myCommand.ExecuteNonQuery();
                }

                //int rowsAffected = myCommand.ExecuteNonQuery();
                //myCommand.Parameters.Clear();
                //myCommand.CommandText = idSql;  
                //myCommand.CommandType = CommandType.Text;

                if (currentTransaction == null)
                {
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                writeLog("0", "executeInsertSQL error", e, sqlQ, dbparams);
                if (raise) { throw; }
            }
            finally
            {
                myCommand.Dispose();
            }
            return identity;
        }

        //############################################################################################################
        // DataReader
        //############################################################################################################
        /// <summary>
        /// returns iterable collection (yield return) for better memory consumption but:
        ///      - there is no cache support 
        ///      - it may throw (ignores the raise field) so you need to provide your own try/cacth
        ///      - it does not automatically rollback transactions in case of errors
        /// </summary>
        public virtual IEnumerable<T> getFromReader<T>(string sqlQ, Func<DbDataReader, T> getObjectFromReader) { return getFromReader<T>(sqlQ, null, CommandType.Text, getObjectFromReader); }
        public virtual IEnumerable<T> getFromReader<T>(string sqlQ, IDBParams dbparams, Func<DbDataReader, T> getObjectFromReader) { return getFromReader<T>(sqlQ, dbparams, CommandType.Text, getObjectFromReader); }
        public virtual IEnumerable<T> getFromReader<T>(string sqlQ, IDBParams dbparams, CommandType cmdType, Func<DbDataReader, T> getObjectFromReader)
        {
            lock (this) 
            {
                return _getFromReader<T>(sqlQ, dbparams, cmdType, getObjectFromReader);
            }                                               
        }
        protected IEnumerable<T> _getFromReader<T>(string sqlQ, IDBParams dbparams, CommandType cmdType, Func<DbDataReader, T> getObjectFromReader)
        {
            using (DbCommand myCommand = this.getDataCommand(sqlQ))
            {
                this.doSqlTrace(sqlQ, dbparams);

                myCommand.CommandTimeout = this.commandTimeOut;
                myCommand.CommandType = cmdType;
                if (this.currentTransaction != null)
                {
                    myCommand.Transaction = this.currentTransaction;
                }
                myCommand.Parameters.Clear();
                if (dbparams != null)
                {
                    foreach (IDBParam p in dbparams)
                    {
                        myCommand.Parameters.Add(p.param);
                    }
                }

                using (var reader = myCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return getObjectFromReader(reader);
                    }
                }
            }
        }


    }


}





