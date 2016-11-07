using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Util.TickerService
{
    public class TimeOfDayProcessingStrategy : IProcessingStrategy
    {
        private DateTime lastProcessTime;
        private readonly DateTime timeToProcess;

        public TimeOfDayProcessingStrategy(DateTime timeToProcess)
        {
            this.timeToProcess = timeToProcess;                        
        }
       
        public double TimerTickInterval { get { return 10; } }      //10 secods


        public bool CanProcess
        {
            get
            {
                DateTime dt = DateTime.Now;
                if (dt.TimeOfDay >= timeToProcess.TimeOfDay)
                {
                    //has not been processed before, so go ahead
                    if (lastProcessTime == default(DateTime))
                    {
                        return true;
                    }
                    //has been processed before, so we need to make sure this is 24 hours later
                    if (dt.Day != lastProcessTime.Day ||
                        dt.Month != lastProcessTime.Month ||
                        dt.Year != lastProcessTime.Year)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void SetProcessingFlag()
        {
            this.lastProcessTime = DateTime.Now;
        }    


    }
}
