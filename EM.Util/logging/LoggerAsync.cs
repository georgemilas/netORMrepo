using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EM.Collections;
using EM.Collections.ThreadQueue;

namespace EM.Logging
{
    public class LoggerAsync : Logger
    {
        public LoggerAsync() : base() { initThread(); }
        public LoggerAsync(string logName) : base(logName) { initThread(); }

        private ActionQueueThread thread;
        private void initThread()
        {
            this.thread = new ActionQueueThread();
        }

        private void setThreadId(ref string msg, ref string moreDetails)
        {
            string tid = "Thread Id: " + Thread.CurrentThread.ManagedThreadId;
            if (msg == null && moreDetails == null)
            {
                msg = tid;
            }
            else
            {
                if (moreDetails == null || moreDetails.Trim() == "") { msg = tid + " " + msg; }
                else { moreDetails = tid + StringUtil.CRLF + moreDetails; }
            }            
        }
        private object _lockSync = new object();
        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails)
        {
            thread.enqueue(delegate 
            {
                setThreadId(ref msg, ref moreDetails);
                base.write(logId, logLevel, msg, e, moreDetails);                
            });
        }

        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(string logId, ILogLevel logLevel, string msg, string moreDetails)
        {
            thread.enqueue(delegate
            {
                setThreadId(ref msg, ref moreDetails);
                base.write(logId, logLevel, msg, moreDetails);                
            }); 
        }

        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(ILogMessage message)
        {
            thread.enqueue(delegate
            {
                string msg = message.msg;
                string moreDetails = message.moreDetails;
                setThreadId(ref msg, ref moreDetails);  //can't pass a property as a ref 
                message.msg = msg;
                message.moreDetails = moreDetails;
                base.write(message);                
            });             
        }

        /// <summary>
        /// the last method and in the chain of calls 
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            thread.enqueue(delegate
            {
                setThreadId(ref msg, ref moreDetails);
                base.write(appId, logId, logLevel, msg, e, moreDetails, context);                
            });             
        }

        public override void Dispose()
        {
            this.thread.Dispose();
            base.Dispose();
        }
        
    }
}
