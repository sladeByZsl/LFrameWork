using System.Linq;
using System.Collections.Generic;

//LRU缓存

namespace LFrameWork.AssetManagement
{
    public class LRU_1
    {
        private LinkedList<string> list = new LinkedList<string>();
        private Dictionary<string, LinkedListNode<string>> dict = new Dictionary<string, LinkedListNode<string>>();
        public int size { get; set; }
        private int usedSize;
        private Queue<LinkedListNode<string>> gcPool = new Queue<LinkedListNode<string>>();

        public LRU_1(int size)
        {
            this.size = size;
        }

        public void Put(string key)
        {
            if (dict.TryGetValue(key, out var node))
            {
                //已经包含了，移动到第一个位
                list.Remove(node);
                list.AddFirst(node);
            }
            else
            {
                //不存在，增加一个
                LinkedListNode<string> newNode = GetNode(key);
                list.AddFirst(newNode);
                dict.Add(key, newNode);
                if (usedSize >= size)
                {
                    //已经满了，移除最后一个
                    var last = list.Last;
                    dict.Remove(last.Value);
                    list.RemoveLast();
                    ReleaseNode(last);
                }
                else
                {
                    usedSize++;
                }
            }
        }

        public bool Contains(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool HasSpace()
        {
            return usedSize < size;
        }

        private LinkedListNode<string> GetNode(string key)
        {
            if (gcPool.Count > 0)
            {
                var node = gcPool.Dequeue();
                node.Value = key;
                return node;
            }

            return new LinkedListNode<string>(key);
        }

        private void ReleaseNode(LinkedListNode<string> node)
        {
            gcPool.Enqueue(node);
        }

        public string[] ToArray()
        {
            return list.ToArray();
        }
    }

    //LRU-K
    public class LRUCache
    {
        private LRU_1 cache;

        private class HistoryCounter
        {
            public string key;
            public int count;
        }
        private Dictionary<string, LinkedListNode<HistoryCounter>> historyDict = new Dictionary<string, LinkedListNode<HistoryCounter>>();
        private LinkedList<HistoryCounter> historyCounters = new LinkedList<HistoryCounter>();
        private int historySize;
        private int usedHistorySize;

        private Queue<LinkedListNode<HistoryCounter>> gcPool = new Queue<LinkedListNode<HistoryCounter>>();

        private int k;

        public LRUCache(int cacheSize)
        {
            cache = new LRU_1(cacheSize);
            historySize = cacheSize * 3;
            this.k = 2;
        }

        public LRUCache(int cacheSize, int historySize, int k)
        {
            cache = new LRU_1(cacheSize);
            this.historySize = historySize;
            this.k = k;
        }

        public void EnsureSize(int size)
        {
            if (cache.size < size)
            {
                cache.size = size;
            }
            historySize = cache.size * 3;
        }

        public void Put(string key)
        {
            if (cache.HasSpace())
            {
                //有空间，直接缓存了
                cache.Put(key);
                return;
            }

            if (historyDict.TryGetValue(key, out var historyNode))
            {
                //在历史记录里，检查使用次数
                historyNode.Value.count++;
                if (historyNode.Value.count >= k)
                {
                    //historyDict.Remove(key);
                    //historyCounters.Remove(historyNode);
                    cache.Put(key);
                }
                else
                {
                    historyCounters.Remove(historyNode);
                    historyCounters.AddFirst(historyNode);
                }
            }
            else
            {
                //不存在，增加一个
                LinkedListNode<HistoryCounter> newNode = GetNode(key);
                historyCounters.AddFirst(newNode);
                historyDict.Add(key, newNode);
                if (usedHistorySize >= historySize)
                {
                    //已经满了，移除最后一个
                    var last = historyCounters.Last;
                    historyDict.Remove(last.Value.key);
                    historyCounters.RemoveLast();
                    ReleaseNode(last);
                }
                else
                {
                    usedHistorySize++;
                }
            }
        }

        public bool Contains(string key)
        {
            return cache.Contains(key);
        }

        private LinkedListNode<HistoryCounter> GetNode(string key)
        {
            if (gcPool.Count > 0)
            {
                var node = gcPool.Dequeue();
                node.Value.key = key;
                node.Value.count = 1;
                return node;
            }

            return new LinkedListNode<HistoryCounter>(new HistoryCounter() { key = key, count = 1 });
        }

        private void ReleaseNode(LinkedListNode<HistoryCounter> node)
        {
            gcPool.Enqueue(node);
        }

        public string[] ToArray()
        {
            return cache.ToArray();
        }
    }
}



