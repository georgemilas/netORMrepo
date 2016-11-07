using System;
using System.ServiceModel;

namespace EM.Util.WCF
{
    /// <summary>
    /// Don't do using instead inastatiate and then call method in this class  
    /// using(T client = new T) 
    /// {
    ///      t.someMethod();
    /// }
    /// </summary>    
    public class WCFClientFacade<T> where T : ICommunicationObject                                        
    {

        /*
         
         Here is more detail from the referenced "interal discussion", which provides some historical context as well.  I think a key thing to notice is that Close() often implies doing "real work" that may fail, including network communication handshakes to shutdown sessions, committing transactions, etc.
         See also http://windowssdk.msdn.microsoft.com/en-us/library/aa355056.aspx
         
          ICommunicationObject (from which ServiceHost, ClientBase, IChannel, IChannelFactory, and IChannelListener ultimately derive) has always had two methods for shutting down the object: (a) Close, and (b) Abort.  The semantics are that if you want to shutdown gracefully, call Close otherwise to shutdown ungracefully you call Abort.  
          As a consequence, Close() takes a Timeout and has an async version (since it can block), and also Close() can throw Exceptions. Documented Exceptions out of Close are CommunicationException (of which CommunicationObjectFaultedException is a subclass), and TimeoutException. 
          Abort() conversely is not supposed to block (or throw any expected exceptions), and therefore doesn’t have a timeout or an async version.
          These two concepts have held from the inception of Indigo through today. So far, so good. 
          In its original incarnation, ICommunicationObject : IDisposable.  As a marker interface, we thought it would be useful to notify users that the should eagerly release this object if possible. This is where the problems begin.  
          Until Beta 1, we had Dispose() == Abort().  Part of the reasoning was that Dispose() should do the minimum necessary to clean up.  This was possibly our #1 complaint in Beta 1. Users would put their channel in a using() block, and any cached messages waiting to be flushed would get dropped on the floor. Transactions wouldn’t get committed, sessions would get ACKed, etc.
          Because of this feedback, in Beta 2 we changed our behavior to have Dispose() ~= Close(). We knew that throwing causes issues (some of which are noted on this thread), so we made Dispose try to be “smart”. That is, if we were not in the Opened state, we would under the covers call Abort(). This has its own set of issues, the topmost being that you can’t reason about the system from a reliability perspective. Dispose can still throw, but it won’t _always_ notify you that something went wrong.  Ultimately we made the decision that we needed to remove IDisposable from ICommunicationObject.  After much debate, IDisposable was left on ServiceHost and ClientBase, the theory being that for many users, it’s ok if Dispose throws, they still prefer the convenience of using(), and the marker that it should be eagerly cleaned up.  You can argue (and some of us did) that we should have removed it from those two classes as well, but for good or for ill we have landed where we have. It’s an area where you will never get full agreement, so we need to espouse best practices in our SDK samples, which is the try{Close}/catch{Abort} paradigm.
  
         */

        public static R doEngineCallWithDispose<R>(T client, Func<T, R> call)
        {
            return doEngineCallWithDispose(client, () => call(client));
        }
        public static R doEngineCallWithDispose<R>(T client, Func<R> call) 
        {
            try
            {
                return call();
            }
            finally
            {
                // timeouts settings etc: http://blogs.msdn.com/drnick/archive/2006/03/10/547568.aspx
                // why try/close/catch/abort: http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/b95b91c7-d498-446c-b38f-ef132989c154
                try { client.Close(); }
                catch
                {
                    client.Abort();

                    //http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/770ba6c2-cc19-4336-bc09-53d5750105d3/
                    //try { c.ChannelFactory.Close(); }
                    //catch { c.ChannelFactory.Abort(); }
                }
            }
        }

        public static void doEngineCallWithDispose(T client, Action<T> call)
        {
            doEngineCallWithDispose(client, () => call(client));
        }
        public static void doEngineCallWithDispose(T client, Action call)
        {
            try
            {
                call();
            }
            finally
            {
                // timeouts settings etc: http://blogs.msdn.com/drnick/archive/2006/03/10/547568.aspx
                // why try/close/catch/abort: http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/b95b91c7-d498-446c-b38f-ef132989c154
                try { client.Close(); }
                catch
                {
                    client.Abort();

                    //http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/770ba6c2-cc19-4336-bc09-53d5750105d3/
                    //try { c.ChannelFactory.Close(); }
                    //catch { c.ChannelFactory.Abort(); }
                }
            }
        }

    }
}
