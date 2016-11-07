using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Util.TickerService
{
    public class TimeIntervalProcessingStrategy : IProcessingStrategy
    {
        private DateTime lastProcessTime;
        //protected ServiceLogger log { get; set; }

        public TimeIntervalProcessingStrategy(double processingSecondsToWait)
        {
            //this.log = ServiceLogger.instance;
            this.ProcessingSecondsToWait = processingSecondsToWait;                        
        }
       
        public double TimerTickInterval { get { return this.ProcessingSecondsToWait; } }

        /// <summary>
        /// this is the actual processing interval and it may be diferent then the ticker interval
        /// </summary>
        protected double ProcessingSecondsToWait { get; private set; }

        public bool CanProcess
        {
            get
            {
                return this.secondsElapsed >= this.ProcessingSecondsToWait;                
            }
        }

        public void SetProcessingFlag()
        {
            this.lastProcessTime = DateTime.Now;
        }


        /// <summary>
        /// Total seconds elapsed since the last processing step was performed (ProcessServiceTick)
        /// </summary>
        protected double secondsElapsed
        {
            get
            {
                return (DateTime.Now - this.lastProcessTime).TotalSeconds;
            }
        }


    }
}
