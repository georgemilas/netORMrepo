using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EM.Collections;


namespace EM.Logging
{
    /// <summary>
    /// - log to a file
    /// - if file path is not given then writes to appId_yyyy-mm-dd.log in current folder
    /// </summary>
    public class FileLogger : BaseLogger, IDisposable
    {
        protected FileLogger() { }
        public FileLogger(string appId, Level level) : this(appId, new LogLevel(level)) { }
        public FileLogger(string appId, ILogLevel level): this(appId, level, null) {}
        public FileLogger(string appId, Level level, string filePath) : this(appId, new LogLevel(level), filePath) { }
        public FileLogger(string appId, ILogLevel level, string filePath)
        {
            this.appId = appId;
            this.level = level;
            if (filePath != null && filePath.Trim()!="")
            {
                this.filePath = filePath;
            }
            else
            {
                this.filePath = Environment.CurrentDirectory + "\\" + appId + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            }            
        }
        
        protected FileStream fs;
        protected DateTime createFs; 

        protected virtual void createFileIfNedded()
        {
            if (!File.Exists(this.filePath))
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                fs = new FileStream(this.filePath, FileMode.Create);
                string s = this.headerTemplate();
                fs.Write(ascii.GetBytes(s + StringUtil.CRLF), 0, s.Length + 1);
                fs.Close();
                fs = null;
            }            
                        
            if (fs == null)
            {
                fs = new FileStream(this.filePath, FileMode.Open);
                createFs = DateTime.Now;
            }            
            
        }

        private LogDetailTemplate _detailTemplate = LogTemplates.TableStyleTemplateFull2;
        public virtual LogDetailTemplate detailTemplate
        {
            get { return _detailTemplate; }
            set { _detailTemplate = value; }
        }

        private LogHeaderTemplate _headerTemplate = LogTemplates.BasicTableStyleTemplateHeader;
        public virtual LogHeaderTemplate headerTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; }
        }

        private string _filePath;
        public virtual string filePath
        {
            get { return this._filePath; }
            set
            {
                this._filePath = value;
            }
        }

        
        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            lock (this)
            {
                if (fs == null)
                {
                    this.createFileIfNedded();
                }
                else
                {
                    if ((DateTime.Now - createFs).TotalHours > 8)
                    {
                        fs.Close();
                        fs = null;
                        this.createFileIfNedded();
                    }
                }
            }
            
            lock (fs)
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                //FileStream fs;

                //this.createFileIfNedded();

                //if (File.Exists(this.filePath))
                    //fs = new FileStream(this.filePath, FileMode.Open);
                //else
                //    fs = new FileStream(this.filePath, FileMode.Create);

                string s = this.detailTemplate(appId, logId, logLevel, msg, getDetails(e, moreDetails));

                fs.Position = fs.Length;
                fs.BeginWrite(
                        ascii.GetBytes(s + StringUtil.CRLF), 
                        0, 
                        s.Length + 1, 
                        new AsyncCallback((ar) =>
                            {
                                fs.EndWrite(ar);
                                fs.Flush();  
                            }), 
                        null
                );
                //fs.Flush();
                //fs.Close();
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
        }

        #endregion
    }
}
