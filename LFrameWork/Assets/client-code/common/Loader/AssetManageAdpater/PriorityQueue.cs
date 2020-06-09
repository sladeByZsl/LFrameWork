using System;
using System.Collections.Generic;

namespace LFrameWork.AssetManagement
{
    public class PriorityQueue<T>
    {
        private SortedList<int, Queue<T>> pq;

        public PriorityQueue()
        {
            pq = new SortedList<int, Queue<T>>();
        }

        public void Enqueue(int priority, T obj)
        {
            Queue<T> q;
            if (!pq.TryGetValue(priority, out q))
            {
                q = new Queue<T>();
                pq.Add(priority, q);
            }
            q.Enqueue(obj);
        }

        public T Dequeue()
        {
            MakeValid();
            if (pq.Count <= 0)
            {
                throw new Exception("priority queue is empty.");
            }
            var q = pq.Values[0];
            if (q.Count <= 0)
            {
                pq.RemoveAt(0);
            }
            return q.Dequeue();
        }

        public T Peek()
        {
            MakeValid();
            if (pq.Count <= 0)
            {
                throw new Exception("priority queue is empty.");
            }
            var q = pq.Values[0];
            return q.Peek();
        }

        private void MakeValid()
        {
            while (pq.Count > 0)
            {
                var q = pq.Values[0];
                if (q.Count <= 0)
                {
                    pq.RemoveAt(0);
                    continue;
                }
                return;
            }
        }

        public void Clear()
        {
            pq.Clear();
        }

        public bool IsEmpty()
        {
            MakeValid();
            return pq.Count == 0;
        }
    }

}

