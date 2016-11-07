using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EM.Collections;
using System.Collections.Concurrent;

namespace EM.Collections.ThreadQueue
{

    /// <summary>
    /// a new thread is constatnly pooling a queue for new items to run and runs them
    /// </summary>
    public class ActionQueueThread: IDisposable
    {
        private Thread thread;

        private BlockingCollection<Action> _queue;
        
        public ActionQueueThread() 
        { 
            this.run(); 
        }
                

        public void run()
        {
            this._queue = new BlockingCollection<Action>();
            thread = new Thread(loop);
            thread.IsBackground = true;
            thread.Start();            
        }

        public void stop()
        {
            if (thread != null)
            {
                thread.Abort();
                while (thread.IsAlive)
                {
                    Thread.SpinWait(1);
                }
                thread = null;
            }
        }

        /// <summary>
        /// adds Action to the queue (throws NullReferenceException if thread is not running)
        /// </summary>
        public void enqueue(Action action)
        {
            if (thread != null)
            {
                this._queue.Add(action);                
            }
            else
            {
                throw new NullReferenceException("Thread was not yet initialized, call the run method before adding to the queue");
            }
        }
        
        private void loop()
        {
            try
            {
                while (true)
                {                    
                    Action action = this._queue.Take();
                    if (action != null)
                    {
                        runAction(action);
                    }                    
                }
            }
            catch (ThreadAbortException)
            {
                //okidoki
            }
        }


        protected virtual void runAction(Action action)
        {
            //exceptions during BeginInvoke are catched and are thrown when calling EndInvoke 
            try
            {
                action();
            }
            catch (ThreadAbortException e) { throw e; }
            catch (Exception)
            {
                //eat up and continue
                //the passed in delegate should catch all errors and handle them 
            }            
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.stop();
            this._queue.Dispose();                        
        }

        #endregion
    }
}
