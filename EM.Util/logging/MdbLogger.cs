using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using EM.Collections;
//using EDB;


namespace EM.Logging
{

    //public class MdbLogger: BaseLogger 
    //{
    //    private ODBCWorker db;
    //    private string _mdbPath;

    //    public MdbLogger(string appId)
    //    {
    //        this.appId = appId;
    //        this.mdbPath = Environment.CurrentDirectory + "\\ELog.mdb";
    //    }

    //    public string mdbPath 
    //    {
    //        get { return this._mdbPath; }
    //        set 
    //        {
    //            this._mdbPath = value;
    //            this.db = new ODBCWorker("Driver={Microsoft Access Driver (*.mdb)};DBQ=" + this.mdbPath);
    //            //this.db = new ODBCWorker();
    //            //this.db.CONN_STR = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + this.mdbPath;
    //            //this.db.init();
    //        }
    //    }

    //    /// <summary>
    //    /// generic log write method
    //    /// </summary>
    //    public override void write(string appId, string logId, string logLevel, string msg, string stackTrace)
    //    {
    //        string sqlQ = "INSERT INTO tbl_log_main (fld_app_id, fld_log_id, fld_msg, fld_stack_trace, fld_level) VALUES (@app, @log_id, @msg, @stack, @level)";
    //        DBParams prs = new DBParams();
    //        prs.Add(new DBParam("@app", appId));
    //        prs.Add(new DBParam("@log_id", logId));
    //        prs.Add(new DBParam("@msg", msg));
    //        prs.Add(new DBParam("@level", logLevel));
    //        prs.Add(new DBParam("@stack", stackTrace != null ? stackTrace : ""));

    //        this.db.executeQuery(sqlQ, true, prs);            
    //    }

    //    /*
    //    public void write(string logId, string logLevel, string msg, string stackTrace)
    //    {
    //        OdbcConnection o_sqlcn = new OdbcConnection();
    //        o_sqlcn.ConnectionString = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + this.mdbPath;
    //        string sqlQ = String.Format("INSERT INTO tbl_log_main (fld_app_id, fld_log_id, fld_msg, fld_stack_trace, fld_log_level) VALUES ('{0}', '{1}', {2})", appId, msg.Replace("'", "''"), bCancel);

    //        try
    //        {
    //            o_sqlcn.Open();
    //            OdbcCommand myCommand = new OdbcCommand(sqlQ, o_sqlcn);
    //            myCommand.CommandTimeout = 60;
    //            myCommand.CommandType = CommandType.Text;
    //            myCommand.ExecuteNonQuery();
    //            o_sqlcn.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            WriteToFile(String.Format("\n---------------\nID {0}, ERR {1}\nDB_LOG_Err {2}", appId, msg, e.ToString()));
    //        }
    //        if (o_sqlcn.State == ConnectionState.Open) o_sqlcn.Close();
    //    }
    //    */

    //}



}
