using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using EM.Collections;
using EM.DB;
//using EM.DB;


namespace EM.Logging
{
    public class DatabaseLogger : BaseLogger
    {
        public ILoggingDB db;
        public string logTableName;

        public DatabaseLogger(string appId, ILoggingDB db, string fullLogTableName, Level level) : this(appId, db, fullLogTableName, new LogLevel(level)) { }
        public DatabaseLogger(string appId, ILoggingDB db, string fullLogTableName, ILogLevel level)
        {
            this.appId = appId;
            this.db = db;
            this.logTableName = fullLogTableName;
            this.level = level;
        }

        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            string sqlQ = "INSERT INTO " + this.logTableName + " (fld_app_id, fld_log_id, fld_msg, fld_stack_trace, fld_level) VALUES (@app, @log_id, @msg, @stack, @level)";
            Dictionary<string, object> prs = new Dictionary<string, object>();
            prs.Add("@app", appId);
            prs.Add("@log_id", logId);
            prs.Add("@msg", msg);
            prs.Add("@level", logLevel.ToString());
            prs.Add("@stack", getDetails(e, moreDetails));

            this.db.executeQuery(sqlQ, prs);
        }


        public virtual bool createLogTable() { return this.createLogTable(2000); }
        public virtual bool createLogTable(int version)
        {   //this creates the table on an sql server 2005 only - nvarchar(max) instead on ntext
            try
            {
                List<string> lst = new List<string>();
                Console.WriteLine("\r\n\r\n--------------------\r\nCHECK EXISTS SQL LOGGING TABLE\r\n--------------------\r\n\r\n");
                DataTable dt = this.db.getDataTable("select name from dbo.sysobjects where OBJECTPROPERTY(id, N'IsUserTable') = 1");
                foreach (DataRow row in dt.Rows)
                {
                    lst.Add(row["name"].ToString().ToLower().Trim());
                }
                if (!lst.Contains("tbl_application_log"))
                {
                    Console.WriteLine("\r\n\r\n--------------------\r\nCREATE SQL LOGGING TABLE\r\n--------------------\r\n\r\n");
                    string sqlQ = null;
                    switch (version)
                    {
                        case 2005:
                            sqlQ = "CREATE TABLE " + this.logTableName + @" (
                                              [id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
                                              [fld_app_id] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_log_id] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_msg] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_stack_trace] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_level] [nvarchar](10) NULL,  --INFO, ERR, WARN, DEBUG, INVOICE_CANCEL
                                              [fld_datetime] [datetime] NOT NULL CONSTRAINT [DF_tbl_application_log_fld_datetime]  DEFAULT (getdate()),
                                               CONSTRAINT [PK_tbl_application_log] PRIMARY KEY CLUSTERED 
                                                (
                                                    [id] ASC
                                                )WITH (IGNORE_DUP_KEY = OFF) ON [DATA]
                                            ) ON [DATA]";
                            break;
                        default:    //sql version 2000
                            sqlQ = "CREATE TABLE " + this.logTableName + @" (
                                          [id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
                                              [fld_app_id] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_log_id] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_msg] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_stack_trace] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                              [fld_level] [nvarchar](10) NULL,  --INFO, ERR, WARN, DEBUG, INVOICE_CANCEL
                                              [fld_datetime] [datetime] NOT NULL CONSTRAINT [DF_tbl_application_log_fld_datetime]  DEFAULT (getdate()),
                                           CONSTRAINT [PK_tbl_application_log] PRIMARY KEY CLUSTERED ([id] ASC) ON [DATA]
                                        ) ON [DATA]";
                            break;
                    }

                    this.db.executeQuery(sqlQ);
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("--------------------------------------------------->");
                Console.WriteLine("SQL LOG TABLE NOT EXIST & CAN'T BE CREATED (=>using DefaultStackedLogger.WriteToMdb)\r\n{0}", e.ToString());
                Console.WriteLine("<---------------------------------------------------");
                return false;
            }
        }


    }

}

