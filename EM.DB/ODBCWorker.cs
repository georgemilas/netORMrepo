using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Configuration;
using EM.Collections;
using EM.Logging;
using EM.Cache;

namespace EM.DB
{
    /// <summary>
    /// - ODBC Server Class (see BaseDBWorker for details)
    /// - cmdTimeOut and nr_try_count have no effect in the ODBC version of getDataTable, getDataSet, addToDataSet
    /// </summary>
    public class ODBCWorker : BaseDBWorker, IDisposable
    {
        protected OdbcConnection sqlConn;
         
        public ODBCWorker(): base() { }
        public ODBCWorker(string connStr): base(connStr) { }
        public ODBCWorker(EDictionary<string, string> config): base(config) { }
        public ODBCWorker(OdbcConnection conn)
        {
            this.sqlConn = conn;
            this.uniqueQueryID = "-1";
            this.CONN_STR = this.sqlConn.ConnectionString;
            this.cache = null;
            this.raise = true;
        }

        public override void init()
        {
            this.sqlConn = new System.Data.Odbc.OdbcConnection();
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
            get { return this.sqlConn; }
        }

        public override DbDataAdapter getDataAdapter()
        {
            return new OdbcDataAdapter();
        }
        public override DbCommand getDataCommand(string sqlStr)
        {
            return new OdbcCommand(sqlStr, (OdbcConnection)this.connection);
        }

        public override DBParams getNewDBParams()
        {
            return new DBParams(typeof(OdbcParameter));
        }

        /// <summary>
        ///  - creates a connection string to be used for a System.Data.Odbc.OdbcConnection
        ///  - the "other" parrameter may be null or a string with other parameters (ex: "Port=1234;Options=-1234")
        ///  - the driver parameter must be between curly-braces (ex: "{MySQL ODBC 3.51 Driver}" )
        /// </summary>
        public static string MakeConnectionString(string server, string database, string user, string password, string driver, string other) {
            if (other == null) other = "";
            return String.Format("Database={0};Server={1};Uid={2};Pwd={3};Driver={4};{5}", database, server, user, password, driver, other); 
        }

        /// <summary>
        ///  - cmdTimeOut and nr_try_count have no effect in the ODBC version of getDataTable, getDataSet, addToDataSet
        /// </summary>
        public override DataTable getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.getDataTable(sqlQ, dbparams, cmdType);
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.ErrorCode);
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

        /// <summary>
        ///  - cmdTimeOut and nr_try_count have no effect in the ODBC version of getDataTable, getDataSet, addToDataSet
        /// </summary>
        public override DataSet getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.getDataSet(sqlQ, dbparams, cmdType);
            }
            catch (OdbcException e)
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

        /// <summary>
        ///  - cmdTimeOut and nr_try_count have no effect in the ODBC version of getDataTable, getDataSet, addToDataSet
        /// </summary>
        public override DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            try
            {
                return base.addToDataSet(ref ds, tableName, sqlQ, dbparams, cmdType);
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.ErrorCode);
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


    }










}
