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
    /// - Sql Server Class (see BaseDBWorker for details)
    /// - may not use a contructor but the factory method (GetWorker) below, so that it returns the corect class instance based of config 
    ///   parameters (ex. to use Worker or NetworkAwareWorker?)
    /// </summary>
    public class SqlServerDBWorker : BaseDBWorkerConnectionPerCommand, IDisposable
    {
        protected SqlConnection sqlConn; 
        public SqlServerDBWorker(): base() { }
        public SqlServerDBWorker(string connStr): base(connStr) {}
        public SqlServerDBWorker(EDictionary<string, string> config): base(config)  {}
        public SqlServerDBWorker(SqlConnection conn)
        {
            this.sqlConn = conn;
            this.uniqueQueryID = "-1";
            this.CONN_STR = this.sqlConn.ConnectionString;
            this.cache = null;
            this.raise = true;
        }

        public override void init()
        {
            this.sqlConn = new System.Data.SqlClient.SqlConnection();
            this.sqlConn.ConnectionString = this.CONN_STR;
            this.sqlConn.Open();  //let it throw
            if (this.contextCommand != null)
            {
                this.contextCommand = this.contextCommand;  //add the context into the database again
            }
        }
        
        public override void Dispose()
        {
            if (this.sqlConn != null && this.sqlConn.State == ConnectionState.Open)
            {
                this.sqlConn.Close();
                this.sqlConn = null;
            }
        }

        public override DbConnection connection
        {
            get 
            {
                //if ( this.sqlConn == null || 
                //    ( this.sqlConn != null && 
                //         ( this.sqlConn.State== ConnectionState.Closed || 
                //           this.sqlConn.State == ConnectionState.Broken 
                //          ) 
                //    ) 
                //   )
                //{
                //    this.init();
                //}
                return this.sqlConn; 
            }
        }

        public override DbDataAdapter getDataAdapter()
        {
            return new SqlDataAdapter();
        }
        public override DbCommand getDataCommand(string sqlStr)
        {
            this.restoreConnection();
            return new SqlCommand(sqlStr, (SqlConnection)this.connection);
        }

        public override DBParams getNewDBParams()
        {
            return new DBParams(typeof(SqlParameter));
        }

        protected override void setToCache(string key, Object val, DbCommand command) 
        {
            if (this.cache != null)
            {
                CacheDependency dep = DepCache.getCacheDedendency(this.CONN_STR, (SqlCommand) command);
                CacheValue<string> cv = new CacheValue<string>(val, this.cache.time_out_period, new CacheDependencyWrapper(dep));
                this.cache.set(key, cv);
            }
        }
       
        public static string MakeConnectionString(string server, string database, string user, string password) { return String.Format("Initial Catalog={0};Data Source={1};User ID={2};Password={3};", database, server, user, password); }
        public static string MakeConnectionString(string server, string database, string user, string password, int timeout) { return String.Format("Initial Catalog={0};Data Source={1};User ID={2};Password={3};Connect Timeout={4};", database, server, user, password, timeout); }
        /// <summary>
        /// make a trusted connection ( windows authentication )
        /// </summary>
        public static string MakeConnectionString(string server, string database, int timeout) { return String.Format("Trusted_Connection=Yes;data source={1};persist security info=False;initial catalog={0};Connect Timeout={2};", database, server, timeout); }
        /// <summary>
        /// make a trusted connection ( windows authentication )
        /// </summary>
        public static string MakeConnectionString(string server, string database) { return String.Format("Trusted_Connection=Yes;data source={1};persist security info=False;initial catalog={0};", database, server); }

        public static SqlServerDBWorker GetWorker() { return GetWorker(null); }
        public static SqlServerDBWorker GetWorker(EDictionary<string, string> settings) { return GetWorker(settings, false); }
        public static SqlServerDBWorker GetWorker(EDictionary<string, string> settings, bool netAware)
        {
            if (netAware)
            {
                //Console.WriteLine("using NetworkAwareDBWorker - several db connections");
                return new NetworkAwareDBWorker(settings);
            }
            else
            {
                //Console.WriteLine("using SqlServerDBWorker  - one db connection");
                SqlServerDBWorker dbw = new SqlServerDBWorker(settings);
                dbw.init();
                return dbw;
            }
        }


        public override DataTable getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.getDataTable(sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message + StringUtil.CRLF + " Err Nr: " + e.Number);
                if (this.raise)
                {
                    throw e;
                }
                return new DataTable();
            }
            catch (Exception e)
            {
                if (this.raise)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return new DataTable();
            }

        }

        public override DataSet getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.getDataSet(sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.ErrorCode);
                if (this.raise)
                {
                    throw e;
                }
                return new DataSet();
            }
            catch (Exception e)
            {
                if (this.raise)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return new DataSet();
            }

        }

        public override DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.addToDataSet(ref ds, tableName, sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.Number);
                if (this.raise)
                {
                    throw e;
                }
                return ds;
            }
            catch (Exception e)
            {
                if (this.raise)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return ds;
            }


        }

        /// <summary>
        /// executes an Insert SQL Statement and return the id of the row inserted
        ///     - if errors occure, by default dosn't raise them but logs the errors and returns -1 as identity
        ///     - if erros shoud be raised use the overloaded version executeInsertSql(sql, true)
        /// </summary>
        public override int executeInsertSql(string sqlQ) { return executeInsertSql(sqlQ, this.raise, new SQLServerLastInserIDProvider()); }
        public override int executeInsertSql(string sqlQ, IDBParams dbparams) { return executeInsertSql(sqlQ, this.raise, dbparams, new SQLServerLastInserIDProvider()); }
        
    }

    


    





}
