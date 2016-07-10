using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOIBC
{
    public class FixedSizedQueue<T>
    {
        private readonly ConcurrentQueue<T> _q = new ConcurrentQueue<T>();

        public int Limit { get; set; }

        public delegate void ObjectEnqueue(T enqueued);
        public event ObjectEnqueue ObjectEnqueueEvent;

        public delegate void ObjectDequeue(T dequeued);
        public event ObjectDequeue ObjectDequeueEvent;

        public FixedSizedQueue(int limit)
        {
            Limit = limit;
        }

        public IEnumerable<T> AsEnumerable()
        {
            return _q.AsEnumerable();
        }

        public void Enqueue(T obj)
        {
            _q.Enqueue(obj);

            ObjectEnqueueEvent?.Invoke(obj);

            lock (this)
            {
                T overflow;
                while (_q.Count > Limit && _q.TryDequeue(out overflow))
                {
                    ObjectDequeueEvent?.Invoke(overflow);
                }
            }
        }

    }
}
