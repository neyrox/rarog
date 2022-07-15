using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;

namespace Engine.Storage
{
    public static class StreamStorage
    {
        private const int FormatVersion = 0;

        public static byte[] NextPage(Stream stream)
        {
            var buf = new byte[PageDesc.Size];
            stream.Read(buf, 0, buf.Length);
            return buf;
        }
        
        public static void StoreColumnMeta(Stream stream, Column column)
        {
            var buf = new byte[1024];
            int offset = 0;
            BytePacker.PackSInt32(buf, FormatVersion, ref offset);
            BytePacker.PackString8(buf, column.Name, ref offset);
            BytePacker.PackString8(buf, column.TypeNameP, ref offset);
            stream.Write(buf, 0, offset);
        }

        public static Column LoadColumnMeta(Stream stream, string tablePath)
        {
            // TODO: reformat
            var buf = new byte[1024];
            stream.Read(buf, 0, (int)Math.Min(stream.Length, buf.Length));
            int offset = 0;
            var formatVersion = BytePacker.UnpackSInt32(buf, ref offset);
            if (formatVersion != FormatVersion)
                return null;

            var name = BytePacker.UnpackString8(buf, ref offset);
            var columnType = BytePacker.UnpackString8(buf, ref offset);

            Console.WriteLine($"Loading column {name} of type {columnType}");

            switch (columnType)
            {
                case "Int":
                    return new ColumnInteger(tablePath, name);
                case "Dbl":
                    return new ColumnDouble(tablePath, name);
                case "Str":
                    return new ColumnVarChar(tablePath, name);
            }

            return null;
        }

        public static IReadOnlyDictionary<long, int> SelectInts(Stream stream, ConditionInteger cond, int limit)
        {
            var result = new Dictionary<long, int>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                var idxValues = IntPage.Load(header, page);

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

        public static IReadOnlyDictionary<long, double> SelectDoubles(Stream stream, ConditionDouble cond, int limit)
        {
            var result = new Dictionary<long, double>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                var idxValues = DoublePage.Load(header, page);

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

        public static IReadOnlyDictionary<long, string> SelectVarChars(Stream stream, ConditionString cond, int limit)
        {
            var result = new Dictionary<long, string>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                var idxValues = VarCharPage.Load(header, page);

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

        public static IReadOnlyDictionary<long, int> SelectInts(Stream stream, SortedSet<long> indices)
        {
            var result = new Dictionary<long, int>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);

                if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                        continue;

                var idxValues = IntPage.Load(header, page);
                foreach (var iv in idxValues)
                {
                    if (indices.Contains(iv.Key))
                        result.Add(iv.Key, iv.Value);
                }
            }

            return result;
        }

        public static IReadOnlyDictionary<long, double> SelectDoubles(Stream stream, SortedSet<long> indices)
        {
            var result = new Dictionary<long, double>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);

                if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxValues = DoublePage.Load(header, page);
                foreach (var iv in idxValues)
                {
                    if (indices.Contains(iv.Key))
                        result.Add(iv.Key, iv.Value);
                }
            }

            return result;
        }

        public static IReadOnlyDictionary<long, string> SelectVarChars(Stream stream, SortedSet<long> indices)
        {
            var result = new Dictionary<long, string>();
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                if (indices == null)
                {
                    var idxValues = VarCharPage.Load(header, page);
                    foreach (var iv in idxValues)
                        result.Add(iv.Key, iv.Value);
                }
                else
                {
                    if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                        continue;

                    var idxValues = VarCharPage.Load(header, page);
                    foreach (var iv in idxValues)
                    {
                        if (indices.Contains(iv.Key))
                            result.Add(iv.Key, iv.Value);
                    }
                }
            }

            return result;
        }

        public static void UpdateInts(Stream stream, long idx, int val)
        {
            if (!FindPage(stream, idx, out var header, out var page))
                return;

            var idxVals = IntPage.Load(header, page);
            idxVals[idx] = val;
            // TODO: reuse buffer
            page = IntPage.Serialize(idxVals);

            WriteBack(stream, page);
        }

        public static void UpdateDoubles(Stream stream, long idx, double val)
        {
            if (!FindPage(stream, idx, out var header, out var page))
                return;

            var idxVals = DoublePage.Load(header, page);
            idxVals[idx] = val;
            // TODO: reuse buffer
            page = DoublePage.Serialize(idxVals);

            WriteBack(stream, page);
        }

        public static void UpdateVarChars(Stream stream, long idx, string val)
        {
            if (!FindPage(stream, idx, out var header, out var page))
                return;

            var idxVals = VarCharPage.Load(header, page);
            idxVals[idx] = val;
            // TODO: reuse buffer
            page = VarCharPage.Serialize(idxVals);

            WriteBack(stream, page);
        }

        public static void InsertInts(Stream stream, long idx, int val)
        {
            // TODO: handle split
            SortedDictionary<long, int> idxVals;
            if (FindPageForInsert(stream, idx, out var header, out var page))
            {
                idxVals = IntPage.Load(header, page);
                stream.Seek(-PageDesc.Size, SeekOrigin.Current);
            }
            else
            {
                idxVals = new SortedDictionary<long, int>();
            }

            // TODO: reuse buffer
            idxVals.Add(idx, val);
            page = IntPage.Serialize(idxVals);
            Write(stream, page);
        }

        public static void InsertDoubles(Stream stream, long idx, double val)
        {
            // TODO: handle split
            SortedDictionary<long, double> idxVals;
            if (FindPageForInsert(stream, idx, out var header, out var page))
            {
                idxVals = DoublePage.Load(header, page);
                stream.Seek(-PageDesc.Size, SeekOrigin.Current);
            }
            else
            {
                idxVals = new SortedDictionary<long, double>();
            }

            // TODO: reuse buffer
            idxVals.Add(idx, val);
            page = DoublePage.Serialize(idxVals);
            Write(stream, page);
        }

        public static void InsertVarChars(Stream stream, long idx, string val)
        {
            // TODO: handle split
            SortedDictionary<long, string> idxVals;
            if (FindPageForInsert(stream, idx, out var header, out var page))
            {
                idxVals = VarCharPage.Load(header, page);
                stream.Seek(-PageDesc.Size, SeekOrigin.Current);
            }
            else
            {
                idxVals = new SortedDictionary<long, string>();
            }

            // TODO: reuse buffer
            idxVals.Add(idx, val);
            page = VarCharPage.Serialize(idxVals);
            Write(stream, page);
        }

        public static void DeleteInts(Stream stream, SortedSet<long> indices)
        {
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxVals = IntPage.Load(header, page);
                foreach (var idx in indices)
                {
                    idxVals.Remove(idx);
                }
                // TODO: reuse buffer
                page = IntPage.Serialize(idxVals);
                WriteBack(stream, page);
                return;
            }
        }

        public static void DeleteDoubles(Stream stream, SortedSet<long> indices)
        {
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxVals = DoublePage.Load(header, page);
                foreach (var idx in indices)
                {
                    idxVals.Remove(idx);
                }
                // TODO: reuse buffer
                page = DoublePage.Serialize(idxVals);
                WriteBack(stream, page);
                return;
            }
        }

        public static void DeleteVarChars(Stream stream, SortedSet<long> indices)
        {
            while (stream.Position < stream.Length)
            {
                var page = NextPage(stream);
                var header = new PageHeader(page);
                if (!Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max))
                    continue;

                var idxVals = VarCharPage.Load(header, page);
                foreach (var idx in indices)
                {
                    idxVals.Remove(idx);
                }
                // TODO: reuse buffer
                page = VarCharPage.Serialize(idxVals);
                WriteBack(stream, page);
                return;
            }
        }

        private static bool FindPage(Stream stream, long idx, out PageHeader header, out byte[] page)
        {
            header = null;
            page = null;

            while (stream.Position < stream.Length)
            {
                page = NextPage(stream);
                header = new PageHeader(page);
                if (header.Include(idx))
                    return true;
            }

            return false;
        }
        
        private static bool FindPageForInsert(Stream stream, long idx, out PageHeader header, out byte[] page)
        {
            header = null;
            page = null;

            if (stream.Length < PageDesc.Size)
                return false;

            // Always last
            // TODO: rework?
            stream.Seek(-PageDesc.Size, SeekOrigin.End);
            page = NextPage(stream);
            header = new PageHeader(page);

            return true;
        }


        public static void Write(Stream fs, byte[] buf)
        {
            fs.Write(buf, 0, buf.Length);
        }

        public static void WriteBack(Stream stream, byte[] page)
        {
            stream.Seek(-PageDesc.Size, SeekOrigin.Current);
            stream.Write(page, 0, page.Length);
        }

        public static bool Overlap(long start1, long end1, long start2, long end2)
        {
            return end1 >= start2 && end2 >= start1;
        }
    }
}
