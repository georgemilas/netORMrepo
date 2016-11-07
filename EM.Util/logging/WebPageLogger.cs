using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Web.UI;
using EM.Collections;
//using EM.DB;


namespace EM.Logging
{
    public class WebPageLogger : BaseLogger
    {
        public Page page;

        public WebPageLogger(string appId, Level level, Page page): this(appId, new LogLevel(level), page) { }
        public WebPageLogger(string appId, ILogLevel level, Page page)
        {
            this.appId = appId;
            this.page = page;
            this.level = level;
        }
      
        public LogDetailTemplate logTemplate = LogTemplates.BasicHTMLTemplate;
        
        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            string s = this.logTemplate(appId, logId, logLevel, msg, getDetails(e, moreDetails));
            page.Response.Write(s);
        }
        
    }
}
