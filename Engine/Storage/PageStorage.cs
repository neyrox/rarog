using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;
using Engine.Statement;

namespace Engine.Storage
{
    public abstract class PageStorage<T> where T: IComparable<T>
    {
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

        public IReadOnlyDictionary<long, T> Select(Stream stream, SortedSet<long> indices)
        {
            var result = new Dictionary<long, T>();
            while (stream.Position < stream.Length)
            {
                var buffer = StreamStorage.NextPage(stream);
                var header = new PageHeader(buffer);

                if (!StreamStorage.Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxValues = Load(header, buffer);

                foreach (var iv in idxValues)
                {
                    if (indices.Contains(iv.Key))
                        result.Add(iv.Key, iv.Value);
                }

                if (result.Count == indices.Count)
                    return result;
            }

            return result;
        }

        public IReadOnlyDictionary<long, T> Select(Stream stream, Condition<T> cond, int limit)
        {
            var result = new Dictionary<long, T>();
            while (stream.Position < stream.Length)
            {
                var page = StreamStorage.NextPage(stream);
                var header = new PageHeader(page);
                var idxValues = Load(header, page);

                foreach (var iv in idxValues)
                {
                    if (cond.Satisfies(iv.Value))
                        result.Add(iv.Key, iv.Value);

                    if (limit > 0 && result.Count >= limit)
                        return result;
                }
            }

            return result;
        }

        public void Update(Stream stream, long idx, OperationGeneric<T> op)
        {
            if (!StreamStorage.FindPage(stream, idx, out var header, out var page))
                return;

            var idxVals = Load(header, page);
            idxVals[idx] = op.Perform(idxVals[idx]);
            // TODO: reuse buffer
            page = Serialize(idxVals, out var tail);
            StreamStorage.WriteBack(stream, page);
            AppendTail(stream, tail);
        }

        public void Insert(Stream stream, long idx, T val)
        {
            SortedDictionary<long, T> idxVals;
            if (StreamStorage.FindPageForInsert(stream, idx, out var header, out var page))
            {
                idxVals = Load(header, page);
                stream.Seek(-Page.Size, SeekOrigin.Current);
            }
            else
            {
                idxVals = new SortedDictionary<long, T>();
            }

            // TODO: reuse buffer
            idxVals.Add(idx, val);
            page = Serialize(idxVals, out var tail);
            StreamStorage.Write(stream, page);
            AppendTail(stream, tail);
        }

        public void Delete(Stream stream, SortedSet<long> indices)
        {
            while (stream.Position < stream.Length)
            {
                var page = StreamStorage.NextPage(stream);
                var header = new PageHeader(page);
                // Read only headers before going further
                if (!StreamStorage.Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxVals = Load(header, page);
                foreach (var idx in indices)
                    idxVals.Remove(idx);

                // TODO: reuse buffer
                page = Serialize(idxVals, out var tail);
                StreamStorage.WriteBack(stream, page);
                return;
            }
        }

        private void AppendTail(Stream stream, SortedDictionary<long, T> tail)
        {
            if (tail != null)
            {
                var extraPage = Serialize(tail, out var tailAgain);
                stream.Seek(0, SeekOrigin.End);
                StreamStorage.Write(stream, extraPage);
            }
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
            var header = new PageHeader(minIdx, maxIdx, idxVals.Count);
            header.Serialize(buffer);

            return buffer;
        }

        protected abstract T UnpackValue(byte[] buffer, ref int offset);
        protected abstract void PackValue(byte[] buffer, T value, ref int offset);
        protected abstract int CalcMaxPairSize(T value);
    }
}