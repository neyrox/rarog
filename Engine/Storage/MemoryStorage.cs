using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private readonly Dictionary<string, long> tableNextIdx = new Dictionary<string, long>();

        public void StoreTableMeta(string fileName, long nextIdx)
        {
            var tableName = fileName.Split('.')[0];
            tableNextIdx[tableName] = nextIdx;
        }

        public void LoadTableMeta(string fileName, out long nextIdx)
        {
            var tableName = fileName.Split('.')[0];
            nextIdx = tableNextIdx[tableName];
        }

        public void StoreColumnMeta(Column column, string tableName, string fileName)
        {
            using (var stream = PrepareStream(fileName))
            {
                StreamStorage.StoreColumnMeta(stream, column);
                buffers[fileName] = stream.ToArray();
            }
        }

        public Column LoadColumnMeta(string tablePath, string fileName, Registry registry)
        {
            using (var stream = PrepareStream(fileName))
            {
                return StreamStorage.LoadColumnMeta(stream, tablePath, registry);
            }
        }

        public string[] GetTableNames()
        {
            return tableNextIdx.Keys.ToArray();
        }

        public string[] GetColumnFiles(string tableDir)
        {
            var result = new SortedSet<string>();
            foreach (var fb in buffers)
            {
                if (fb.Key.StartsWith(tableDir))
                    result.Add(fb.Key);
            }

            return result.ToArray();
        }

        public void CreateDirectoryIfNotExist(string tableDir)
        {
        }

        public void DeleteFile(string fileName)
        {
            buffers.Remove(fileName);
            var lastDot = fileName.LastIndexOf('.');
            var tableName = fileName.Substring(0, lastDot);
            tableNextIdx.Remove(tableName);
        }

        public void DeleteDirectory(string tableDir)
        {
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
