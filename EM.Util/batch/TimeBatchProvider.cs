using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Batch
{
    /// <summary>
    /// flush batch collection each time when a specified time interval has elapsed
    /// </summary>
    public class TimeBatchProvider: BatchProvider
    {
        public TimeSpan waitingPeriod;
        public DateTime lastFlushTime;

        public TimeBatchProvider(int waitingPeriodInMinutes) : this(TimeSpan.FromMinutes(waitingPeriodInMinutes)) { }
        public TimeBatchProvider(TimeSpan waitingPeriod)
            : base()
        {
            if (waitingPeriod >= TimeSpan.FromMinutes(1))
            {
                this.waitingPeriod = waitingPeriod;
            }
            else
            {
                throw new IndexOutOfRangeException("A waiting period must be any time spaning more the 1 minute");
            }

            this.lastFlushTime = DateTime.Now;
        }
        
        public override bool couldFlush
        {
            get 
            { 
                TimeSpan elapsed = DateTime.Now - this.lastFlushTime;
                return elapsed >= this.waitingPeriod;
            }
        }

        public override void flush()
        {
            base.flush();
            this.lastFlushTime = DateTime.Now;
        }

    }
}
