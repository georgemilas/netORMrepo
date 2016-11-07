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
    /// a new thread is constatnly pooling a queue for new items to run and runs them async
    /// </summary>
    public class ActionAsyncQueueThread: ActionQueueThread
    {
        public ActionAsyncQueueThread() : base() 
        { 
            
        }
        
        protected override void runAction(Action action)
        {
            action.BeginInvoke(delegate(IAsyncResult ar)
            {
                //exceptions during BeginInvoke are catched and are thrown when calling EndInvoke 
                try
                {
                    action.EndInvoke(ar);
                }
                catch (ThreadAbortException e) { throw e; }
                catch (Exception)
                {
                    //eat up and continue
                    //the passed in delegate should catch all errors and handle them 
                }
            }, null);
        }

    }
}
