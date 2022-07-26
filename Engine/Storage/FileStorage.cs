using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;

namespace Engine.Storage
{
    public class FileStorage : IStorage, IStreamProvider
    {
        private string dataPath;

        public FileStorage(string dataPath)
        {
            this.dataPath = dataPath;
        }

        public void StoreTableMeta(string fileName, long nextIdx)
        {
            using (var file = File.Open(Path.Combine(dataPath, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                var buffer = new byte[256];
                int offset = 0;
                BytePacker.PackSInt64(buffer, nextIdx, ref offset);
                file.Write(buffer, 0, offset);
            }
        }

        public void LoadTableMeta(string fileName, out long nextIdx)
        {
            using (var file = File.Open(Path.Combine(dataPath, fileName), FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[256];
                file.Read(buffer, 0, (int) Math.Min(buffer.Length, file.Length));
                int offset = 0;
                nextIdx = BytePacker.UnpackSInt64(buffer, ref offset);
            }
        }

        public void StoreColumnMeta(Column column, string tableName, string fileName)
        {
            using (var file = File.Open(Path.Combine(dataPath, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamStorage.StoreColumnMeta(file, column);
            }
        }

        public Column LoadColumnMeta(string tablePath, string fileName, Registry registry)
        {
            using (var file = File.Open(Path.Combine(dataPath, fileName), FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.LoadColumnMeta(file, tablePath, registry);
            }
        }

        public string[] GetTableNames()
        {
            var tableEntries = Directory.GetDirectories(dataPath);

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
            var fullTableDir = Path.Combine(dataPath, tableDir);
            if (!Directory.Exists(fullTableDir))
            {
                Console.WriteLine($"Table directory {fullTableDir} doesn't exist");
                return new string[0];
            }

            var result = new List<string>();

            var columnEntries = Directory.GetFiles(fullTableDir);
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
            var fullTableDir = Path.Combine(dataPath, tableDir);

            if (!Directory.Exists(fullTableDir))
                Directory.CreateDirectory(fullTableDir);
        }

        public void DeleteFile(string fileName)
        {
            var fullPath = Path.Combine(dataPath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public void DeleteDirectory(string tableDir)
        {
            var fullPath = Path.Combine(dataPath, tableDir);

            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath);
        }

        public Stream OpenRead(string name)
        {
            var fullPath = Path.Combine(dataPath, name);

            try
            {
                Console.WriteLine($"Opening {fullPath} to read");
                return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public Stream OpenReadWrite(string name)
        {
            var fullPath = Path.Combine(dataPath, name);

            try
            {
                Console.WriteLine($"Opening {fullPath} to read and write");
                return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
