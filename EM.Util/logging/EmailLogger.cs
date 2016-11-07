using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EM.Collections;
using EM.Util;
using System.Net.Mail;
using EM.Batch;


namespace EM.Logging
{
    public class EmailLogger : BaseLogger, IDisposable
    {
        public string fromAdr;
        public string toAdr;

        private string _mailServer;

        public Type GetType()
        {
            return this.GetType();

        }

        public string mailServer
        {
            get{ return _mailServer; }
            set
            {
                _mailServer = value;
                this.mailer = new Mailer(_mailServer);
            }
        }
        

        public Mailer mailer { get; protected set; }

        public EmailLogger(string appId, string fromAdr, string toAdr, string mailServer, Level level): this(appId, fromAdr, toAdr, mailServer, new LogLevel(level)) { }
        public EmailLogger(string appId, string fromAdr, string toAdr, string mailServer, ILogLevel level)
        {
            this.appId = appId;
            this.mailServer = mailServer;
            this.fromAdr = fromAdr;
            this.toAdr = toAdr;
            this.level = level;
        }

        public LogDetailTemplate bodyTemplate = LogTemplates.BasicEmailBodyTemplate;
        public LogDetailTemplate subjectTemplate = LogTemplates.BasicEmailSubjectTemplate;

        private IBatchProvider _batch;
        public IBatchProvider batch
        {
            get { return this._batch; }
            set 
            { 
                this._batch = value;
                this._batch.FlushEvent += new FlushEventHandler(_batch_FlushEvent);
            }
        }

        void _batch_FlushEvent(object source, List<object> content)
        {
            if (content.Count > 0)
            {
                EList<object> batchContent = (EList<object>)content;
                string subject = this.appId + " log messages";
                string body = batchContent.join("\n\r");
                this.mailer.send(this.mailer.getMail(this.fromAdr, this.toAdr, subject, body));
            }
        }
        

        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            //from,    to,     subject,   body
            
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            string details = getDetails(e, moreDetails);
            string subject = this.subjectTemplate(appId, logId, logLevel, msg, details);
            string body = this.bodyTemplate(appId, logId, logLevel, msg, details); 
            mailer.logger.appId = this.appId;
            if (this.batch != null)
            {
                this.batch.add(body);
            }
            else
            {
                mailer.send(mailer.getMail(this.fromAdr, this.toAdr, subject, body));
            }
        }





        #region IDisposable Members

        public void Dispose()
        {
            if (this.batch != null)
            {
                this.batch.Dispose();
            }
        }

        #endregion
    }
}
