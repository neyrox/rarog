using System;
using System.Collections.Generic;

namespace Engine.Storage
{
    public interface ICached
    {
        string Id { get; }
        bool Evict();

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

        public bool Evict()
        {
            // TODO: Flush?
            if (Dirty)
                return false;

            Data = null;

            return true;
        }
    }

    public class CacheHost
    {
        private const int MaxPages = 1000;
        private readonly object syncObject = new object();
        private readonly LinkedDictionary<string, ICached> pq = new LinkedDictionary<string, ICached>();

        public void Up(ICached page)
        {
            page.Validate();

            lock (syncObject)
            {
                //Console.WriteLine($"Using page {page.Id}");

                if (pq.Contains(page.Id))
                {
                    pq.Remove(page.Id);
                    pq.AddFirst(page.Id, page);
                }
                else
                {
                    pq.AddFirst(page.Id, page);
                    
                    if (pq.Count > MaxPages)
                    {
                        var oldestPage = pq.Last;
                        if (oldestPage.Value.Evict())
                        {
                            pq.Remove(oldestPage.Key);
                        }
                    }
                }
            }
        }

        public void Remove(string id)
        {
            lock (syncObject)
            {
                pq.Remove(id);
            }
        }

        public int Count
        {
            get
            {
                lock (syncObject)
                {
                    return pq.Count;
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