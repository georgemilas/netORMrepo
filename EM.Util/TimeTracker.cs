using System;
using System.Collections.Generic;
using System.Text;
using EM.Logging;
using EM.Collections;

namespace EM.Util
{
    public class TimeTracker
    {

        public static TimeSpan trackTimeForAction(Action action)
        {
            DateTime n = DateTime.Now;
            action();
            return DateTime.Now - n;
        }


        protected ILogger log { get; set; }
        protected DateTime lastTrackTime { get; set; }
        protected string method { get; set; }

        protected EDictionary<string, DateTime> traces;

        /// <summary>
        /// initializez lastTraceRun to DateTime.Now using message "START"
        /// </summary>
        public TimeTracker(string method, ILogger log)
        {
            this.method = method;
            this.log = log;
            this.lastTrackTime = DateTime.Now;
            this.traces = new EDictionary<string, DateTime>();
            traces.Add("START", lastTrackTime);
            this.enableOutput = true;
        }

        public bool enableOutput { get; set; }

        public DateTime startTime
        {
            get { return this.traces["START"]; }
        }

        public void resetTraceStartTime()
        {
            this.lastTrackTime = DateTime.Now;
        }

        /// <summary>
        /// log time for MSG starting from the tracker initialization
        /// </summary>
        public void timeSinceStart(string msg)
        {
            trackTime(this.startTime, msg);
        }

        /// <summary>
        /// log time for MSG starting from the trace was done for msgFrom 
        /// or if it can't find that trace it will track using last trace run 
        /// </summary>
        public void timeSinceOtherTrack(string msgFrom, string msg)
        {
            trackTime(this.traces.get(msgFrom, this.lastTrackTime), msg);
        }

        /// <summary>
        /// log time for MSG starting from last trace done
        /// </summary>
        public void timeSinceLastTrack(string msg)
        {
            trackTime(this.lastTrackTime, msg);
        }

        protected void trackTime(DateTime lastTraceTime, string msg)
        {
            //message like: _setCompanyPayrollData - from dataset to employees loop 
            if (this.enableOutput)
            {
                this.putOutput(msg, lastTraceTime);
            }
            this.lastTrackTime = DateTime.Now;
            traces.Add(msg, this.lastTrackTime);
        }


        protected virtual void putOutput(string msg, DateTime lastTraceTime)
        {
            this.log.debug(" TIME", method + " - " + msg + " took " + (DateTime.Now - lastTraceTime).TotalSeconds.ToString());
        }

        public virtual TimeTracker getNewTracker()
        {
            var tracker  = new TimeTracker(this.method, this.log);
            tracker.enableOutput = this.enableOutput;
            return tracker;
        }


    }
}
