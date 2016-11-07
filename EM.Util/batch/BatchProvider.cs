using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.Batch
{
    public abstract class BatchProvider: IBatchProvider, IDisposable
    {
        protected EList<object> batchContent = new EList<object>();

        public virtual void add(object content)
        {
            this.batchContent.Add(content);
            if (this.couldFlush)
            {
                this.flush();
            }
        }

        public event FlushEventHandler FlushEvent;

        public abstract bool couldFlush { get; }

        public virtual void flush()
        {
            lock (this)
            {
                if (FlushEvent != null)
                {
                    FlushEvent(this, this.batchContent);

                }
                this.batchContent = new EList<object>();
            }
        }



        #region IDisposable Members

        public void Dispose()
        {
            this.flush();
            this.batchContent = null;
        }

        #endregion
    }
}
