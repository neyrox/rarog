using System;
using System.Collections.Generic;
using System.IO;
using Engine.Statement;

namespace Engine.Storage
{
    public class CustomMemoryStream : MemoryStream
    {
        private readonly string name;
        private readonly Dictionary<string, byte[]> buffers;

        public CustomMemoryStream(string name, Dictionary<string, byte[]> buffers)
        {
            this.name = name;
            this.buffers = buffers;
        }

        public override void Close()
        {
            buffers[name] = ToArray();
            base.Close();
        }
    }
    
    public class MemoryStorage : IStorage, IStreamProvider
    {
        private readonly Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();

        private readonly CacheHost cacheHost = new CacheHost();
        private readonly PageStorage<int> intStorage;
        private readonly PageStorage<long> bigIntStorage;
        private readonly PageStorage<double> doubleStorage;
        private readonly PageStorage<string> strStorage;

        private long nextIdx;

        public MemoryStorage()
        {
            intStorage = new IntPage(this, cacheHost);
            bigIntStorage = new  BigIntPage(this, cacheHost);
            doubleStorage = new DoublePage(this, cacheHost);
            strStorage = new VarCharPage(this, cacheHost);
        }

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
            return intStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices)
        {
            return intStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, long> SelectBigInts(string fileName, Condition<long> cond, int limit)
        {
            return bigIntStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, long> SelectBigInts(string fileName, SortedSet<long> indices)
        {
            return bigIntStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, Condition<double> cond, int limit)
        {
            return doubleStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices)
        {
            return doubleStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, Condition<string> cond, int limit)
        {
            return strStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices)
        {
            return strStorage.Select(fileName, indices);
        }

        public void UpdateInts(string fileName, long idx, OperationGeneric<int> op)
        {
            intStorage.Update(fileName, idx, op);
        }

        public void UpdateBigInts(string fileName, long idx, OperationGeneric<long> op)
        {
            bigIntStorage.Update(fileName, idx, op);
        }

        public void UpdateDoubles(string fileName, long idx, OperationGeneric<double> op)
        {
            doubleStorage.Update(fileName, idx, op);
        }

        public void UpdateVarChars(string fileName, long idx, OperationGeneric<string> op)
        {
            strStorage.Update(fileName, idx, op);
        }

        public void InsertInts(string fileName, long idx, int val)
        {
            intStorage.Insert(fileName, idx, val);
        }

        public void InsertBigInts(string fileName, long idx, long val)
        {
            bigIntStorage.Insert(fileName, idx, val);
        }

        public void InsertDoubles(string fileName, long idx, double val)
        {
            doubleStorage.Insert(fileName, idx, val);
        }

        public void InsertVarChars(string fileName, long idx, string val)
        {
            strStorage.Insert(fileName, idx, val);
        }

        public void DeleteInts(string fileName, SortedSet<long> indices)
        {
            intStorage.Delete(fileName, indices);
        }

        public void DeleteBigInts(string fileName, SortedSet<long> indices)
        {
            bigIntStorage.Delete(fileName, indices);
        }

        public void DeleteDoubles(string fileName, SortedSet<long> indices)
        {
            doubleStorage.Delete(fileName, indices);
        }

        public void DeleteVarChars(string fileName, SortedSet<long> indices)
        {
            strStorage.Delete(fileName, indices);
        }

        public void DeleteIntColumn(string fileName)
        {
            intStorage.Delete(fileName);
            buffers.Remove(fileName);
        }

        public void DeleteBigIntColumn(string fileName)
        {
            bigIntStorage.Delete(fileName);
            buffers.Remove(fileName);
        }

        public void DeleteDoubleColumn(string fileName)
        {
            doubleStorage.Delete(fileName);
            buffers.Remove(fileName);
        }

        public void DeleteVarCharColumn(string fileName)
        {
            strStorage.Delete(fileName);
            buffers.Remove(fileName);
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
            buffers.Remove(fileName);
        }

        public void DeleteDirectory(string tableDir)
        {
        }

        public void Flush()
        {
            intStorage.Flush();
            bigIntStorage.Flush();
            doubleStorage.Flush();
            strStorage.Flush();
        }

        private MemoryStream PrepareStream(string fileName)
        {
            var stream = new CustomMemoryStream(fileName, buffers);
            if (!buffers.ContainsKey(fileName))
                return stream;

            var buffer = buffers[fileName];
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public Stream OpenRead(string name)
        {
            return PrepareStream(name);
        }

        public Stream OpenReadWrite(string name)
        {
            return PrepareStream(name);
        }
    }
}