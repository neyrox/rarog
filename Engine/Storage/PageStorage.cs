using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Serialization;
using Engine.Statement;

namespace Engine.Storage
{
    public class PageCache<T>
    {
        public PageHeader Header;

        public SortedDictionary<long, T> Data;
            //new SortedDictionary<long, T>();

        public bool Dirty;
    }

    public class PagesCache<T>
    {
        public SortedList<int, PageCache<T>> Pages =
            new SortedList<int, PageCache<T>>();

        public int PagesCount;
    }

    
    public abstract class PageStorage<T> where T: IComparable<T>
    {
        private readonly SortedList<string, PagesCache<T>> cache =
            new SortedList<string, PagesCache<T>>();

        private IStreamProvider streams;

        protected PageStorage(IStreamProvider streams)
        {
            this.streams = streams;
        }

        public SortedDictionary<long, T> Load(PageHeader header, byte[] buffer)
        {
            var result = new SortedDictionary<long, T>();
            int offset = PageHeader.DataOffset;
            // TODO: compress
            for (int i = 0; i < header.Count; ++i)
            {
                var idx = BytePacker.UnpackSInt64(buffer, ref offset);
                var val = UnpackValue(buffer, ref offset);
                result.Add(idx, val);
            }

            return result;
        }

        public SortedDictionary<long, T> LoadData(PageHeader header, byte[] buffer)
        {
            var result = new SortedDictionary<long, T>();
            int offset = 0;
            // TODO: decompress
            for (int i = 0; i < header.Count; ++i)
            {
                var idx = BytePacker.UnpackSInt64(buffer, ref offset);
                var val = UnpackValue(buffer, ref offset);
                result.Add(idx, val);
            }

            return result;
        }

        public IReadOnlyDictionary<long, T> Select(string name, SortedSet<long> indices)
        {
            var result = new Dictionary<long, T>();
            int pageIdx = -1;
            Stream stream = null;
            try
            {
                while (result.Count < indices.Count)
                {
                    pageIdx++;

                    if (!GetPageCache(name, pageIdx, ref stream, out var pageCache))
                        return result;

                    if (!StreamStorage.Overlap(pageCache.Header.MinIdx, pageCache.Header.MaxIdx, indices.Min, indices.Max))
                    {
                        if (stream != null)
                            StreamStorage.SkipPageData(stream);
                        continue;
                    }

                    LoadPageData(name, pageIdx, pageCache, ref stream);

                    foreach (var iv in pageCache.Data)
                    {
                        if (indices.Contains(iv.Key))
                            result.Add(iv.Key, iv.Value);
                    }
                }

            }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }

            return result;
        }

        public IReadOnlyDictionary<long, T> Select(string name, Condition<T> cond, int limit)
        {
            var result = new Dictionary<long, T>();
            int pageIdx = -1;
            Stream stream = null;

            try
            {
                while (true)
                {
                    pageIdx++;

                    if (!GetPageCache(name, pageIdx, ref stream, out var pageCache))
                        return result;

                    LoadPageData(name, pageIdx, pageCache, ref stream);

                    foreach (var iv in pageCache.Data)
                    {
                        if (cond.Satisfies(iv.Value))
                            result.Add(iv.Key, iv.Value);

                        if (limit > 0 && result.Count >= limit)
                            return result;
                    }
                }
            }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }
        }

        public void Update(string name, long idx, OperationGeneric<T> op)
        {
            int pageIdx = -1;
            Stream stream = null;

            try
            {
                while (true)
                {
                    pageIdx++;
                    if (!GetPageCache(name, pageIdx, ref stream, out var pageCache))
                        return;

                    if (!pageCache.Header.Include(idx))
                        continue;

                    LoadPageData(name, pageIdx, pageCache, ref stream);

                    pageCache.Data[idx] = op.Perform(pageCache.Data[idx]);
                    pageCache.Dirty = true;
                    return;
                }
            }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }
        }

        public void Insert(string name, long idx, T val)
        {
            long maxRowIdx = -1;
            int insPageIdx = -1;
            int tmpPageIdx = -1;
            Stream stream = null;

            try
            {
                while (true)
                {
                    tmpPageIdx++;
                    if (!GetPageCache(name, tmpPageIdx, ref stream, out var tmpPageCache))
                        break;

                    if (tmpPageCache.Header.MaxIdx > maxRowIdx)
                    {
                        maxRowIdx = tmpPageCache.Header.MaxIdx;
                        insPageIdx = tmpPageIdx;
                    }
                }

                if (insPageIdx < 0)
                {
                    insPageIdx = 0;
                    cache[name].Pages.Add(insPageIdx, new PageCache<T>
                    {
                        Header = new PageHeader(idx, idx, 1),
                        Data = new SortedDictionary<long, T> { {idx, val} },
                        Dirty = true
                    });
                    cache[name].PagesCount++;
                    return;
                }

                var pageCache = cache[name].Pages[insPageIdx];

                LoadPageData(name, insPageIdx, pageCache, ref stream);
                pageCache.Data.Add(idx, val);
                pageCache.Header.MinIdx = Math.Min(pageCache.Header.MinIdx, idx);  // Maybe we don't need it?
                pageCache.Header.MaxIdx = Math.Max(pageCache.Header.MaxIdx, idx);
                pageCache.Header.Count++;
                pageCache.Dirty = true;
            }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }
        }

        public void Delete(string name, SortedSet<long> indices)
        {
            int pageIdx = -1;
            Stream stream = null;
            var deleted = new List<long>();

            try
            {
                while (indices.Count > 0)
                {
                    pageIdx++;
                    if (!GetPageCache(name, pageIdx, ref stream, out var pageCache))
                        break;

                    if (!StreamStorage.Overlap(pageCache.Header.MinIdx, pageCache.Header.MaxIdx, indices.Min, indices.Max))
                        continue;

                    LoadPageData(name, pageIdx, pageCache, ref stream);

                    foreach (var idx in indices)
                    {
                        if (pageCache.Data.Remove(idx))
                            deleted.Add(idx);
                    }

                    foreach (var idx in deleted)
                        indices.Remove(idx);
                    deleted.Clear();

                    pageCache.Dirty = true;
                }
            }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }
        }

        public void Flush()
        {
            for (int i = 0; i < cache.Count; i++)
            {
                var name = cache.Keys[i];
                var pagesCache = cache.Values[i];
                Stream stream = null;
                for (int j = 0; j < pagesCache.Pages.Count; j++)
                {
                    var pageCache = pagesCache.Pages.Values[j];
                    if (!pageCache.Dirty)
                        continue;

                    if (stream == null)
                        stream = streams.OpenReadWrite(name);

                    var page = Serialize(pageCache.Data, out var tail);
                    StreamStorage.Write(stream, page, pagesCache.Pages.Keys[j]);
                    pageCache.Dirty = false;
                    AppendTail(stream, pagesCache, tail);
                }

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        private void AppendTail(Stream stream, PagesCache<T> pagesCache, SortedDictionary<long, T> tail)
        {
            if (tail == null)
                return;

            var tailIdx = (int)(stream.Length / Page.Size);
            var header = new PageHeader(tail.Keys.First(), tail.Keys.Last(), tail.Count);
            pagesCache.Pages[tailIdx] = new PageCache<T> { Header = header, Data = tail};
            pagesCache.PagesCount++;

            var extraPage = Serialize(tail, out var tailAgain);
            stream.Seek(0, SeekOrigin.End);
            StreamStorage.Write(stream, extraPage);

            AppendTail(stream, pagesCache, tailAgain);
        }

        private byte[] Serialize(SortedDictionary<long, T> idxVals, out SortedDictionary<long, T> tail)
        {
            tail = null;
            var buffer = new byte[Page.Size];
            long minIdx = long.MaxValue;
            long maxIdx = long.MinValue;
            int offset = PageHeader.DataOffset;
            bool split = false;
            foreach (var idxVal in idxVals)
            {
                if (offset + CalcMaxPairSize(idxVal.Value) >= buffer.Length)
                {
                    split = true;
                    tail = new SortedDictionary<long, T>();
                }

                if (!split)
                {
                    if (idxVal.Key < minIdx)
                        minIdx = idxVal.Key;
                    if (idxVal.Key > maxIdx)
                        maxIdx = idxVal.Key;

                    BytePacker.PackSInt64(buffer, idxVal.Key, ref offset);
                    PackValue(buffer, idxVal.Value, ref offset);
                }
                else
                {
                    tail.Add(idxVal.Key, idxVal.Value);
                }
            }

            if (tail != null)
            {
                // Cut the tail
                foreach (var idxVal in tail)
                    idxVals.Remove(idxVal.Key);
            }
            
            var header = new PageHeader(minIdx, maxIdx, idxVals.Count);
            header.Serialize(buffer);

            return buffer;
        }

        private bool GetPageCache(string name, int pageIdx, ref Stream stream, out PageCache<T> pageCache)
        {
            if (!cache.TryGetValue(name, out var pagesCache))
            {
                pagesCache = new PagesCache<T>();
                cache.Add(name, pagesCache);
            }

            if (!pagesCache.Pages.TryGetValue(pageIdx, out pageCache))
            {
                if (pagesCache.PagesCount > 0 && pageIdx >= pagesCache.PagesCount)
                    return false;

                if (stream == null)
                {
                    stream = streams.OpenRead(name);
                    pagesCache.PagesCount = Math.Max(pagesCache.PagesCount, (int)(stream.Length / Page.Size));
                }

                if ((pageIdx + 1) * (long)Page.Size > stream.Length)
                    return false;

                StreamStorage.SeekToPage(stream, pageIdx);
                var headerBuffer = StreamStorage.ReadHeader(stream);
                var header = new PageHeader(headerBuffer);

                pageCache = new PageCache<T> {Header = header};
                pagesCache.Pages.Add(pageIdx, pageCache);
            }

            return true;
        }

        private void LoadPageData(string name, int pageIdx, PageCache<T> pageCache, ref Stream stream)
        {
            if (pageCache.Data != null)
                return;

            if (stream == null)
                stream = streams.OpenRead(name);

            StreamStorage.SeekToPageData(stream, pageIdx);

            var dataBuffer = StreamStorage.ReadPageData(stream);
            pageCache.Data = LoadData(pageCache.Header, dataBuffer);
        }

        protected abstract T UnpackValue(byte[] buffer, ref int offset);
        protected abstract void PackValue(byte[] buffer, T value, ref int offset);
        protected abstract int CalcMaxPairSize(T value);
    }
}