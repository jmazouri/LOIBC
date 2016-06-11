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

        public FixedSizedQueue(int limit)
        {
            Limit = limit;
        }

        public void Enqueue(T obj)
        {
            _q.Enqueue(obj);
            lock (this)
            {
                T overflow;
                while (_q.Count > Limit && _q.TryDequeue(out overflow)) ;
            }
        }
    }
}
