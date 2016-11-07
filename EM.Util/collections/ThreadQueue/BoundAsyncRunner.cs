using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Security.Permissions;
using System.Timers;
using System.Threading;
using EM.Collection;
using EM.Collections.ThreadQueue;
using System.Threading.Tasks;

namespace EM.Collections.ThreadQueue
{
    /// <summary>
    /// like Parallel.ForEach with maxDegreeOfParallelism but uses action.BeginInvoke instead of the Task library
    /// and guarantees that there are maxDegreeOfParallelism actions executing in parallel at all times 
    /// </summary>    
    public class BoundAsyncRunner<T>
    {
        public int sourceTotal;
        public IList<T> source;
        public Action<T> func;

        private ManualResetEvent mre = new ManualResetEvent(false);
        private int cntProcess = 0;
        private int cntAll = -1;

        /// <summary>
        /// the action provided should catch all errors and handle them otherwise errors will be ignored
        /// to allow remaning actions to run 
        /// </summary>        
        public BoundAsyncRunner(IList<T> source, Action<T> func)
        {
            this.source = source;
            this.sourceTotal = source.Count;
            this.func = func;
        }

        public void run(int upperBound)
        {
            mre = new ManualResetEvent(false);
            int bound = Math.Min(upperBound, sourceTotal);

            for (int i = 0; i < bound; i++)
            {
                Interlocked.Increment(ref cntAll);
                doEnqueue();
            }

            mre.WaitOne();
        }
        private object _lock = new object();
        private void doEnqueue()
        {
            if (cntAll < sourceTotal)
            {
                Interlocked.Increment(ref cntProcess);                
                func.BeginInvoke(source[cntAll], delegate(IAsyncResult ar)
                {
                    //exceptions during BeginInvoke are catched and are thrown when calling EndInvoke 
                    try
                    {
                        func.EndInvoke(ar);
                    }
                    catch
                    {
                        //eat up and continue
                        //the passed in delegate should catch all errors and handle them 
                    }
                    Interlocked.Decrement(ref cntProcess);
                    lock (_lock)
                    {
                        if (cntAll >= sourceTotal && cntProcess == 0)
                        {
                            mre.Set();                                
                        }
                        else
                        {
                            Interlocked.Increment(ref cntAll);
                            doEnqueue();                            
                        }
                    }                                                    
                        
                }, null);                                    
            }
        }


    }

}
