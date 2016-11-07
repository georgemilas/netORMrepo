using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using EM.Collections;


namespace EM.DB
{
    //################################################################################################################
    /// <summary>
    /// NetwarkAwareInvoiceDBWorker - uses new connections for each db query
    /// - does this to attempt to recover from brief network connectivity loss that teoreticaly would leave the main connection unsusable
    /// </summary>
    public class NetworkAwareDBWorker : SqlServerDBWorker
    {
        public NetworkAwareDBWorker(EDictionary<string, string> settings)
            : base(settings) 
        { }

        // the following uses one template method withConnectionDo that uses a delegate 
        // to customize for setNewDataSet, executeQuery, setExtInfo

        protected delegate Object TemplateDelegate();


        public override void Dispose() { }


        //############################################################################################################
        public override DataTable getDataTable(string sqlQ, IDBParams dbparams)
        {
            //Console.WriteLine("Run getDataTable with new connection via delegate");
            return withConnectionDo(delegate()
                    { return base.getDataTable(sqlQ, dbparams); }
                    ) as DataTable;
        }

        public override DataSet getDataSet(string sqlQ, IDBParams dbparams)
        {
            //Console.WriteLine("Run getDataSet with new connection via delegate");
            return withConnectionDo(delegate()
                { return base.getDataSet(sqlQ, dbparams); }
                ) as DataSet;
            
        }
        
        public override DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams)
        {   
            //can't use withConnectionDo because the "ref" keyword is not allowed inside an anonymous delegate
            DataSet res;
            try
            {
                //Console.WriteLine("Run addToDataSet with new connection");
                this.init();
                res = base.addToDataSet(ref ds, tableName, sqlQ, dbparams);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                writeLog("0", "Database Connection Failed", e, "", null);
                res = new DataSet();
            }
            finally
            {
                if (this.sqlConn != null && this.sqlConn.State == ConnectionState.Open)
                {
                    this.sqlConn.Close();
                }
            }

            return res;
        }
        

        public override bool executeQuery(string sqlQ, bool raise, IDBParams dbparams)
        {
            //Console.WriteLine("Run executeQuery with new connection via delegate");
            return (bool)withConnectionDo(delegate()
                { return base.executeQuery(sqlQ, raise, dbparams); }
                );

        }

        //############################################################################################################
        protected Object withConnectionDo(TemplateDelegate td)
        {
            Object res = new Object();

            try
            {
                this.init();
                //Console.WriteLine("Inside withConnectionDo, invoke delegate");
                res = td();  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                writeLog("0", "Database Connection Failed", e, "", null);

            }
            finally
            {
                if (this.sqlConn != null && this.sqlConn.State == ConnectionState.Open)
                {
                    this.sqlConn.Close();
                }
            }

            return res;
        }

    }



}



