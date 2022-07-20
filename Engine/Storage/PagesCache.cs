using System;
using System.Collections.Generic;

namespace Engine.Storage
{
    public interface ICached
    {
        string Id { get; }
        void Evict();

        void Validate();
    }

    public class PageCache<T> : ICached
    {
        public string Id { get; }

        public PageCache(string name, int pageIdx)
        {
            // TODO: use hash instead of string?
            Id = name + pageIdx;
        }

        public PageHeader Header;

        public SortedDictionary<long, T> Data;

        public bool Dirty;

        public void Validate()
        {
            if (Data == null)
                return;

            if (Header.Count != Data.Count)
                throw new Exception("Wrong header");
        }

        public void Evict()
        {
            // TODO: Flush?
            if (Dirty)
                return;
            Data = null;
        }
    }

    public class CacheHost
    {
        private const int MaxPages = 1000;
        private readonly object syncObject = new object();
        private readonly Dictionary<string, ICached> pages = new Dictionary<string, ICached>();
        private readonly SortedList<string, long> priority = new SortedList<string, long>();

        public void Up(ICached page)
        {
            page.Validate();

            lock (syncObject)
            {
                //Console.WriteLine($"Using page {page.Id}");

                if (!pages.ContainsKey(page.Id))
                    pages.Add(page.Id, page);

                priority[page.Id] = DateTime.UtcNow.ToFileTimeUtc();
                
                if (pages.Count <= MaxPages)
                    return;

                // TODO: implement priority queue
                string oldestPageId = priority.Keys[0];
                var oldestTime = priority.Values[0];
                for (int i = 1; i < priority.Count; ++i)
                {
                    if (priority.Values[i] < oldestTime)
                    {
                        oldestPageId = priority.Keys[i];
                        oldestTime = priority.Values[i];
                    }
                }

                var oldestPage = pages[oldestPageId];
                oldestPage.Evict();
                pages.Remove(oldestPageId);
                priority.Remove(oldestPageId);
            }
        }

        public void Remove(string id)
        {
            lock (syncObject)
            {
                pages.Remove(id);
                priority.Remove(id);
            }
        }

        public int Count
        {
            get
            {
                lock (syncObject)
                {
                    return pages.Count;
                }
            }
        }
    }

    public class PagesCache<T>
    {
        public readonly SortedList<int, PageCache<T>> Pages =
            new SortedList<int, PageCache<T>>();

        public int PagesCount;
    }
}