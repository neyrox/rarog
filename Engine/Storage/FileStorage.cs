using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;

namespace Engine.Storage
{
    public class FileStorage : IStorage
    {
        // Full scan
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

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, ConditionInteger cond)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectInts(file, cond);
            }
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, int>();

            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectInts(file, indices);
            }
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, ConditionDouble cond)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectDoubles(file, cond);
            }
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, double>();

            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectDoubles(file, indices);
            }
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, ConditionString cond)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectVarChars(file, cond);
            }
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices)
        {
            if (indices != null && indices.Count == 0)
                return new Dictionary<long, string>();

            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.SelectVarChars(file, indices);
            }
        }

        public void UpdateInts(string fileName, long idx, int val)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.UpdateInts(file, idx, val);
            }
        }

        public void UpdateDoubles(string fileName, long idx, double val)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.UpdateDoubles(file, idx, val);
            }
        }

        public void UpdateVarChars(string fileName, long idx, string val)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.UpdateVarChars(file, idx, val);
            }
        }

        public void InsertInts(string fileName, long idx, int val)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamStorage.InsertInts(file, idx, val);
            }
        }

        public void InsertDoubles(string fileName, long idx, double val)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamStorage.InsertDoubles(file, idx, val);
            }
        }

        public void InsertVarChars(string fileName, long idx, string val)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamStorage.InsertVarChars(file, idx, val);
            }
        }

        public void DeleteInts(string fileName, SortedSet<long> indices)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.DeleteInts(file, indices);
            }
        }

        public void DeleteDoubles(string fileName, SortedSet<long> indices)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.DeleteDoubles(file, indices);
            }
        }

        public void DeleteVarChars(string fileName, SortedSet<long> indices)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                StreamStorage.DeleteVarChars(file, indices);
            }
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
                if (!columnFile.EndsWith(Column.MetaFileExtension))
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
            File.Delete(fileName);
        }
    }
}