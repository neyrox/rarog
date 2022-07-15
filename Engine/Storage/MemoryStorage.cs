using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Storage
{
    public class MemoryStorage : IStorage
    {
        private readonly Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();
        private long nextIdx;

        public void StoreTableMeta(string fileName, long nextIdx)
        {
            this.nextIdx = nextIdx;
        }

        public void LoadTableMeta(string fileName, out long nextIdx)
        {
            nextIdx = this.nextIdx;
        }

        public void StoreColumnMeta(Column column, string tableName, string fileName)
        {
            using (var stream = PrepareStream(fileName))
            {
                StreamStorage.StoreColumnMeta(stream, column);
                buffers[fileName] = stream.ToArray();
            }
        }

        public Column LoadColumnMeta(string tablePath, string fileName)
        {
            using (var stream = PrepareStream(fileName))
            {
                return StreamStorage.LoadColumnMeta(stream, tablePath);
            }
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, Condition<int> cond, int limit)
        {
            using (var stream = PrepareStream(fileName))
            {
                return IntPage.Instance.Select(stream, cond, limit);
            }
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                return IntPage.Instance.Select(stream, indices);
            }
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, Condition<double> cond, int limit)
        {
            using (var stream = PrepareStream(fileName))
            {
                return DoublePage.Instance.Select(stream, cond, limit);
            }
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                return DoublePage.Instance.Select(stream, indices);
            }
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, Condition<string> cond, int limit)
        {
            using (var stream = PrepareStream(fileName))
            {
                return VarCharPage.Instance.Select(stream, cond, limit);
            }
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                return VarCharPage.Instance.Select(stream, indices);
            }
        }

        public void UpdateInts(string fileName, long idx, int val)
        {
            using (var stream = PrepareStream(fileName))
            {
                IntPage.Instance.Update(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void UpdateDoubles(string fileName, long idx, double val)
        {
            using (var stream = PrepareStream(fileName))
            {
                DoublePage.Instance.Update(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void UpdateVarChars(string fileName, long idx, string val)
        {
            using (var stream = PrepareStream(fileName))
            {
                VarCharPage.Instance.Update(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void InsertInts(string fileName, long idx, int val)
        {
            using (var stream = PrepareStream(fileName))
            {
                IntPage.Instance.Insert(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void InsertDoubles(string fileName, long idx, double val)
        {
            using (var stream = PrepareStream(fileName))
            {
                DoublePage.Instance.Insert(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void InsertVarChars(string fileName, long idx, string val)
        {
            using (var stream = PrepareStream(fileName))
            {
                VarCharPage.Instance.Insert(stream, idx, val);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void DeleteInts(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                IntPage.Instance.Delete(stream, indices);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void DeleteDoubles(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                DoublePage.Instance.Delete(stream, indices);
                buffers[fileName] = stream.ToArray();
            }
        }

        public void DeleteVarChars(string fileName, SortedSet<long> indices)
        {
            using (var stream = PrepareStream(fileName))
            {
                VarCharPage.Instance.Delete(stream, indices);
                buffers[fileName] = stream.ToArray();
            }
        }

        public string[] GetTableNames()
        {
            return new string[0];
        }

        public string[] GetColumnFiles(string tableDir)
        {
            return new string[0];
        }

        public void CreateDirectoryIfNotExist(string tableDir)
        {
        }

        public void DeleteFile(string fileName)
        {
        }

        public void DeleteDirectory(string tableDir)
        {
        }

        private MemoryStream PrepareStream(string fileName)
        {
            var stream = new MemoryStream();
            if (!buffers.ContainsKey(fileName))
                return stream;

            var buffer = buffers[fileName];
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}