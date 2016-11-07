using System;

namespace EM.Batch
{
    public interface IBatchProvider: IDisposable
    {
        void add(object content);
        bool couldFlush { get; }
        void flush();
        event FlushEventHandler FlushEvent;
                
    }
}
