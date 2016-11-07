using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using System.Windows.Forms;
using System.Threading;

namespace EM.Collection
{
    /// <summary>
    /// Extension methods for IEnumerable and other static utility methods
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// run func.BeginInvoke ... EndInvoke for each item in the list and returns a list
        /// of the results in the order they were actualy finished
        /// </summary>
        public static IEnumerable<R> ForEachAsync<T, R>(this IEnumerable<T> lst, Func<T, R> func)
        {
            return forEachAsync(func, lst);
        }
        /// <summary>
        /// run action.BeginInvoke ... EndInvoke for each item in the list
        /// </summary>
        public static void ForEachAsync<T>(this IEnumerable<T> lst, Action<T> action)
        {
            forEachAsync(action, lst);
        }

        /// <summary>
        /// run func.BeginInvoke ... EndInvoke for each item in the list and returns a list
        /// of the results in the order they were actualy finished
        /// </summary>
        public static IEnumerable<R> forEachAsync<T, R>(Func<T, R> func, IEnumerable<T> lst)
        {
            //run async
            ManualResetEvent mre = new ManualResetEvent(false);
            List<R> resLst = new List<R>();
            int cnt = 0;
            foreach (T itm in lst)
            {
                Interlocked.Increment(ref cnt);
                func.BeginInvoke(itm, delegate(IAsyncResult ar)
                {
                    R res = default(R);
                    try
                    {
                        res = func.EndInvoke(ar);
                    }
                    catch
                    {
                        //eat up and continue
                        //the passed in delegate should catch all errors and handle them 
                    }                          
                    lock (resLst) { resLst.Add(res); }
                    Interlocked.Decrement(ref cnt);
                    if (cnt == 0) { mre.Set(); }
                }, null);
            }
            mre.WaitOne();
            return resLst;
        }

        /// <summary>
        /// run action.BeginInvoke ... EndInvoke for each item in the list
        /// </summary>
        public static void forEachAsync<T>(Action<T> action, IEnumerable<T> lst)
        {
            //run async
            ManualResetEvent mre = new ManualResetEvent(false);
            int cnt = 0;
            foreach (T itm in lst)
            {
                Interlocked.Increment(ref cnt);
                action.BeginInvoke(itm, delegate(IAsyncResult ar)
                {
                    try
                    {
                        action.EndInvoke(ar);
                    }
                    catch
                    {
                        //eat up and continue
                        //the passed in delegate should catch all errors and handle them 
                    }                    
                    Interlocked.Decrement(ref cnt);
                    if (cnt == 0) { mre.Set(); }
                }, null);
            }
            mre.WaitOne();
        }


    }
}
