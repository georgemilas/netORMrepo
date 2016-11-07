using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EM.Collections;
using System.Collections.Concurrent;

namespace EM.Collections.ThreadQueue
{
    public class NonBlockingActionQueueThread: IDisposable
    {
        private Thread thread;

        private Queue<Action> _queue;

        public NonBlockingActionQueueThread() 
        { 
            this.run(); 
        }
                

        public void run()
        {
            this._queue = new Queue<Action>();
            thread = new Thread(loop);
            thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Lowest;
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
                this._queue.Enqueue(action);                
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
                    if (this._queue.Count > 0)
                    {
                        Action action = null;
                        lock (_queue)
                        {
                            if (this._queue.Count > 0)
                            {
                                action = this._queue.Dequeue();
                            }
                        }

                        if (action != null)
                        {
                            runAction(action);
                        }
                    }
                    else
                    {
                        Thread.Yield();
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
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (e is ThreadAbortException) { throw e; }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.stop();
            this._queue = null;                        
        }

        #endregion
    }
}
