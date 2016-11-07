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
//using System.Web.Caching;

namespace EM.DB
{
    /// <summary>
    /// - Sql Server Class (see BaseDBWorker for details)
    /// - may not use a contructor but the factory method (GetWorker) below, so that it returns the corect class instance based of config 
    ///   parameters (ex. to use Worker or NetworkAwareWorker?)
    /// </summary>
    public class DBWorkerManyTries : SqlServerDBWorker
    {
        public DBWorkerManyTries(): base() { }
        public DBWorkerManyTries(string connStr): base(connStr) {}
        public DBWorkerManyTries(EDictionary<string, string> config): base(config)  {}
        public DBWorkerManyTries(SqlConnection conn): base(conn) {}
        

        public DataTable getDataTable(string sqlQ, int cmdTimeOut, int nr_try_count, DBParams dbparams, CommandType cmdType)
        {
            if (nr_try_count > this.maxNumberOfTries)
            {
                writeLog(this.uniqueQueryID, "Unable to retreive data / Retry = " + nr_try_count , null, "", null);
                //Environment.Exit(10);
            }
            else nr_try_count += 1;

            try
            {
                return base.getDataTable(sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.Number);
                if ((e.Number == -2) && nr_try_count <= this.maxNumberOfTries)
                {
                    Console.WriteLine("getDataTable Retry ...");
                    writeLog(this.uniqueQueryID, "Retry " + nr_try_count, null, "", null);
                    return this.getDataTable(sqlQ, cmdTimeOut + 60, nr_try_count, dbparams, cmdType);
                }
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                return new DataTable();
            }
            catch (Exception e)
            {
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return new DataTable();
            }

        }

        public DataSet getDataSet(string sqlQ, int cmdTimeOut, int nr_try_count, DBParams dbparams, CommandType cmdType)
        {
            if (nr_try_count > this.maxNumberOfTries)
            {
                writeLog(this.uniqueQueryID, "Unable to retreive data / Retry = " + nr_try_count , null, "", null);
                //Environment.Exit(10);
            }
            else nr_try_count += 1;

            try
            {
                return base.getDataSet(sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.ErrorCode);
                if (e.Number == -2 && nr_try_count <= this.maxNumberOfTries)
                {
                    Console.WriteLine("getDataSet Retry ...");
                    writeLog(this.uniqueQueryID, "Retry " + nr_try_count, null, "", null);
                    return this.getDataSet(sqlQ, cmdTimeOut + 60, nr_try_count, dbparams, cmdType);
                }
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                return new DataSet();
            }
            catch (Exception e)
            {
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return new DataSet();
            }

        }

        public DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, int cmdTimeOut, int nr_try_count, DBParams dbparams, CommandType cmdType)
        {
            if (nr_try_count > this.maxNumberOfTries)
            {
                writeLog(this.uniqueQueryID, "Unable to retreive data / Retry = " + nr_try_count, null, "", null);
                //Environment.Exit(10);
            }
            else nr_try_count += 1;

            try
            {
                return base.addToDataSet(ref ds, tableName, sqlQ, dbparams, cmdType);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString() + StringUtil.CRLF + " Err Nr: " + e.Number);
                if (e.Number == -2 && nr_try_count <= this.maxNumberOfTries)
                {
                    Console.WriteLine("addToDataSet Retry ...");
                    writeLog(this.uniqueQueryID, "Retry " + nr_try_count, null, "", null);
                    return this.addToDataSet(ref ds, tableName, sqlQ, cmdTimeOut + 60, nr_try_count, dbparams, cmdType);
                }
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                return ds;
            }
            catch (Exception e)
            {
                if (this.raise || nr_try_count > this.maxNumberOfTries)
                {
                    throw e;
                }
                Console.WriteLine(e.ToString());
                return ds;
            }


        }

        
    }

    


    





}
