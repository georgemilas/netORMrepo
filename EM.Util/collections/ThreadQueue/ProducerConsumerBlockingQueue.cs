using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace EM.Collections.ThreadQueue
{
    //use System.Collections.Concurent.BlockingCollection<T>  instead
    public class ProducerConsumerBlockingQueue<T> : IDisposable
    {
        private Queue<T> _queue = new Queue<T>();

        private bool working = true;

        public void enqueue(T data)
        {
            if (data == null) throw new ArgumentNullException("data");

            if (_queue.Count >= 1)
            {
                _queue.Enqueue(data);
                Monitor.Pulse(_queue);
            }
            else
            {
                lock (_queue)
                {
                    _queue.Enqueue(data);
                    Monitor.Pulse(_queue);
                }
            }
        }
        public T dequeue()
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_queue);
                }
                if (working)
                {
                    return _queue.Dequeue();
                }
                else
                {
                    return default(T);
                }
            }
        }

        public void Dispose()
        {
            lock (_queue)
            {
                _queue.Clear();
                working = false;
                Monitor.PulseAll(_queue);
            }
        }
    }
}
