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
    public abstract class BaseDBWorkerConnectionPerCommand : BaseDBWorker, IDisposable
    {

        public BaseDBWorkerConnectionPerCommand() : base() { }
        public BaseDBWorkerConnectionPerCommand(string connStr): base(connStr) { }
        public BaseDBWorkerConnectionPerCommand(EDictionary<string, string> config): base(config) { }

        /// <summary>
        /// - if current transaction is set then all operations will use it (getDataTable, executeQuery, executeScalar etc...)
        /// - if an error occure during the db opperation, the transaction is rolled back (raises or not based on raiseErrors)
        /// </summary>
        public override DbTransaction currentTransaction
        {
            get { return base.currentTransaction; }
            set 
            { 
                base.currentTransaction = value;
                if (value == null)
                {
                    if (this.connection != null) { this.connection.Close(); }
                }
            }
        }
        
        public override DbTransaction startTransaction() { return this.startTransaction(IsolationLevel.ReadCommitted); }
        public override DbTransaction startTransaction(IsolationLevel level)
        {
            this.restoreConnection();
            DbTransaction trans = this.connection.BeginTransaction(level);
            return trans;
        }

        public override DataTable getSchemaTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._getSchemaTable(sqlQ, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        //############################################################################################################
        // DataTable
        //############################################################################################################
        public override DataTable getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._getDataTable(sqlQ, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                        this.connection.Dispose();
                    }
                }
            }        
        }
                

        //############################################################################################################
        // DataSet
        //############################################################################################################
        public override DataSet getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._getDataSet(sqlQ, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------
        public override DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._addToDataSet(ref ds, tableName, sqlQ, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        //############################################################################################################
        // EXECUTE Query
        //############################################################################################################
        public override bool executeQuery(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._executeQuery(sqlQ, raise, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public override object executeScalar(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType)
        {
            lock (this)
            {
                try
                {
                    return this._executeScalar(sqlQ, raise, dbparams, cmdType);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public override int executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType, ILastInsertIDProvider IdSql)
        {
            lock (this)
            {
                try
                {
                    return this._executeInsertSql(sqlQ, raise, dbparams, cmdType, IdSql);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public override IEnumerable<T> getFromReader<T>(string sqlQ, IDBParams dbparams, CommandType cmdType, Func<DbDataReader, T> getObjectFromReader)
        {
            lock (this)
            {
                try
                {
                    return this._getFromReader(sqlQ, dbparams, cmdType, getObjectFromReader);
                }
                finally
                {
                    if (this.connection != null && this.currentTransaction == null)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

    }


}





