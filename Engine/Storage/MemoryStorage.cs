using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Storage
{
    public class MemoryStorage : IStorage
    {
        private Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();
        
        public Column LoadColumn(string fileName)
        {
            using (var stream = new MemoryStream(buffers[fileName]))
            {
                return StreamStorage.LoadColumn(stream);
            }
        }

        public void Store(ColumnInteger column, string tableDir)
        {
            using (var stream = new MemoryStream())
            {
                StreamStorage.Store(column, stream);
                var key = Column.GetFileName(tableDir, column.Name);
                buffers[key] = stream.ToArray();
            }
        }

        public void Store(ColumnDouble column, string tableDir)
        {
            using (var stream = new MemoryStream())
            {
                StreamStorage.Store(column, stream);
                var key = Column.GetFileName(tableDir, column.Name);
                buffers[key] = stream.ToArray();
            }
        }

        public void Store(ColumnVarChar column, string tableDir)
        {
            using (var stream = new MemoryStream())
            {
                StreamStorage.Store(column, stream);
                var key = Column.GetFileName(tableDir, column.Name);
                buffers[key] = stream.ToArray();
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
    }
}