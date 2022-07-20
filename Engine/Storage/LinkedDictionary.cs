using System.Collections.Generic;

namespace Engine.Storage
{
    public class LinkedDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> dictionary =
            new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        private readonly LinkedList<KeyValuePair<TKey, TValue>> linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();

        public int Count => dictionary.Count;
        public KeyValuePair<TKey, TValue> Last => linkedList.Last.Value;

        public void AddFirst(TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return;

            var node = linkedList.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
            dictionary.Add(key, node);
        }

        public void Remove(TKey key)
        {
            if (!dictionary.TryGetValue(key, out var node))
                return;

            dictionary.Remove(key);
            linkedList.Remove(node);
        }

        public void RemoveLast()
        {
            var node = linkedList.Last;
            linkedList.RemoveLast();

            dictionary.Remove(node.Value.Key);
        }

        public bool Contains(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }
    }
}
