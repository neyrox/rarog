using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;
using Engine.Statement;

namespace Engine.Storage
{
    public class FileStorage : IStorage, IStreamProvider
    {
        private readonly CacheHost cacheHost = new CacheHost();
        private readonly PageStorage<int> intStorage;
        private readonly PageStorage<long> bigIntStorage;
        private readonly PageStorage<double> doubleStorage;
        private readonly PageStorage<string> strStorage;

        public FileStorage()
        {
            intStorage = new IntPage(this, cacheHost);
            bigIntStorage = new  BigIntPage(this, cacheHost);
            doubleStorage = new DoublePage(this, cacheHost);
            strStorage = new VarCharPage(this, cacheHost);
        }

        public void StoreTableMeta(string fileName, long nextIdx)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var buffer = new byte[256];
                int offset = 0;
                BytePacker.PackSInt64(buffer, nextIdx, ref offset);
                file.Write(buffer, 0, offset);
            }
        }

        public void LoadTableMeta(string fileName, out long nextIdx)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[256];
                file.Read(buffer, 0, (int) Math.Min(buffer.Length, file.Length));
                int offset = 0;
                nextIdx = BytePacker.UnpackSInt64(buffer, ref offset);
            }
        }

        public void StoreColumnMeta(Column column, string tableName, string fileName)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamStorage.StoreColumnMeta(file, column);
            }
        }

        public Column LoadColumnMeta(string tablePath, string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.LoadColumnMeta(file, tablePath);
            }
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, Condition<int> cond, int limit)
        {
            return intStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, int>();
            
            return intStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, long> SelectBigInts(string fileName, Condition<long> cond, int limit)
        {
            return bigIntStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, long> SelectBigInts(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, long>();
            
            return bigIntStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, Condition<double> cond, int limit)
        {
            return doubleStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, double>();

            return doubleStorage.Select(fileName, indices);
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, Condition<string> cond, int limit)
        {
            return strStorage.Select(fileName, cond, limit);
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, string>();

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
            DeleteFile(fileName);
        }

        public void DeleteBigIntColumn(string fileName)
        {
            bigIntStorage.Delete(fileName);
            DeleteFile(fileName);
        }

        public void DeleteDoubleColumn(string fileName)
        {
            doubleStorage.Delete(fileName);
            DeleteFile(fileName);
        }

        public void DeleteVarCharColumn(string fileName)
        {
            strStorage.Delete(fileName);
            DeleteFile(fileName);
        }

        public string[] GetTableNames()
        {
            var path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            var tableEntries = Directory.GetDirectories(path);

            var result = new List<string>();

            foreach (var tableEntry in tableEntries)
            {
                if (!tableEntry.EndsWith(Table.TableDirExtension))
                    continue;

                var tableDir = new DirectoryInfo(tableEntry).Name;
                var tableName = tableDir.Substring(0, tableDir.Length - Table.TableDirExtension.Length);
                
                result.Add(tableName);
            }

            return result.ToArray();
        }

        public string[] GetColumnFiles(string tableDir)
        {
            if (!Directory.Exists(tableDir))
            {
                Console.WriteLine($"Table directory {tableDir} doesn't exist");
                return new string[0];
            }

            var result = new List<string>();

            var columnEntries = Directory.GetFiles(tableDir);
            foreach (var columnFile in columnEntries)
            {
                if (!(columnFile.EndsWith(Column.MetaFileExtension) || columnFile.EndsWith(Column.DataFileExtension)))
                    continue;

                result.Add(columnFile);
            }

            return result.ToArray();
        }
        
        public void CreateDirectoryIfNotExist(string tableDir)
        {
            if (!Directory.Exists(tableDir))
                Directory.CreateDirectory(tableDir);
        }

        public void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        public void DeleteDirectory(string tableDir)
        {
            if (Directory.Exists(tableDir))
                Directory.Delete(tableDir);
        }

        public void Flush()
        {
            intStorage.Flush();
            bigIntStorage.Flush();
            doubleStorage.Flush();
            strStorage.Flush();

            Console.WriteLine($"There are {cacheHost.Count} pages in cache");
        }

        public Stream OpenRead(string name)
        {
            try
            {
                Console.WriteLine($"Opening {name} to read");
                return File.Open(name, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public Stream OpenReadWrite(string name)
        {
            try
            {
                Console.WriteLine($"Opening {name} to read and write");
                return File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
