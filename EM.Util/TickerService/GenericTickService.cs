using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Security.Permissions;
using System.Timers;
using EM.Logging;
//using System.Threading;

namespace EM.Util.TickerService
{
    //can not be abstract (Visual Studio can't design it) so we use an interface and make the abstract members part of the interface
    public abstract class GenericTickService : ITickService
    {
        public abstract void ProcessServiceTick();
        public abstract string logID { get; }


        protected Timer timer = new Timer();
        public ILogger log { get; set; }
        
        public IProcessingStrategy ProcessingStrategy { get; protected set; }
        
        public delegate void TickerServiceStopRequest();
        public event TickerServiceStopRequest onStoppingOrPaused;

        protected GenericTickService(IProcessingStrategy processingStrategy) : this(ServiceLogger.instance, processingStrategy) { }
        protected GenericTickService(ILogger log, IProcessingStrategy processingStrategy)
        {
            this.log = log;
            this.ProcessingStrategy = processingStrategy;

            try
            {
                FileIOPermission ioPermission = new FileIOPermission(PermissionState.Unrestricted);
                ioPermission.Demand();

                this.timer.Elapsed += timerTick_Elapsed;
                this.timer.Enabled = false;
                this.timer.Interval = this.ProcessingStrategy.TimerTickInterval * 1000;  //miliseconds
                               
            }
            catch (Exception ex)
            {
                if (this.log != null)
                {
                    this.log.error(this.logID, "Failed to initialize ticker service", ex);
                }
            }
        }

        #region Basic Service Stuff

        public void Start()
        {
            timer.Enabled = true;
            this.log.info(this.logID, "Service Started");
        }

        public void Stop()
        {
            timer.Enabled = false;
            if (this.onStoppingOrPaused != null) { onStoppingOrPaused(); }
            this.log.info(this.logID, "Service Stoped");
        }

        public void Pause()
        {
            timer.Enabled = false;
            if (this.onStoppingOrPaused != null) { onStoppingOrPaused(); }
            this.log.info(this.logID, "Service Paused");
        }

        public void Continue()
        {
            timer.Enabled = true;
            this.log.info(this.logID, "Service Continue");
        }

        public bool EnsureServiceIsRunning()
        {
            if (!timer.Enabled)
            {
                this.Start();
            }
            return true;
        }

        /// <summary>
        /// Make sure the code in ProcessServiceTick takes this in consideration when doing big loops
        /// Additionaly you may also register to be notified for Stop/Pause events via onStoppingOrPaused
        /// </summary>
        public virtual bool ServiceIsStoppingOrPaused
        {
            get { return !timer.Enabled; }
        }

        protected void timerTick_Elapsed(Object sender, System.Timers.ElapsedEventArgs e)
        {
            ClockTick();
        }
        #endregion Basic Service Stuff

        private class TickerProcess
        {
            public bool isProcessing = false;
        }

        private TickerProcess proc = new TickerProcess();
        protected void ClockTick()
        {
            //this.log.debug("Ticker", "Clock Tick");
            try
            {
                if (this.ProcessingStrategy.CanProcess)
                {
                    //this.log.debug("Ticker", string.Format("Start Processing: elapsed {0}, wait {1}", this.secondsElapsed, this.ProcessingSecondsToWait));    
                    this.ProcessingStrategy.SetProcessingFlag();
                    ThreadSafeProcess();
                }
            }
            catch (Exception ex)
            {
                this.log.error(this.logID, "Error calculating tick time elapsed", ex);
            }

        }

        

        private void ThreadSafeProcess()
        {
            String tp = this.GetType().Name;
            tp += String.Format(" ID: {0} ", System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (proc.isProcessing)
            {
                this.log.debug("Ticker", string.Format("{0} is ready to process but another instance of {0} is running so this instance will close", tp));
                return;
            }

            lock (proc)
            {
                if (!proc.isProcessing)
                {
                    //this.lastProcessTime = DateTime.Now;
                    proc.isProcessing = true;
                    this.log.debug("Ticker", string.Format("{0} is waking up and staring to process", tp));
                    try
                    {
                        this.ProcessServiceTick();
                    }
                    catch (Exception ex)
                    {
                        this.log.error(this.logID, tp + "Error processing tick service data", ex);
                    }
                    this.log.debug("Ticker", string.Format("{0} has finished process, now closing", tp));
                    proc.isProcessing = false;
                }
            }
        }
        
    }
}
