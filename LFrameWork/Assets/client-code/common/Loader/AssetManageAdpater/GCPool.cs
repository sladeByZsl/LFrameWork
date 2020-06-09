using System.Collections.Generic;

namespace LFrameWork.AssetManagement
{
    public interface IGCPool
    {
        void Reset();
    }

    public class GCPool<T> where T : IGCPool, new()
    {
        private Queue<T> pool = new Queue<T>();
        private HashSet<T> inPoolSet = new HashSet<T>();

        public T Get()
        {
            if (pool.Count > 0)
            {
                var ins = pool.Dequeue();
                inPoolSet.Remove(ins);
                return ins;
            }
            return new T();
        }

        public void Free(T val)
        {
            if (inPoolSet.Contains(val))
            {
                return;
            }
            val.Reset();
            pool.Enqueue(val);
            inPoolSet.Add(val);
        }
    }
}
