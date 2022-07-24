using System;
using System.Collections.Generic;
using System.IO;
using Engine.Serialization;

namespace Engine.Storage
{
    public class FileStorage : IStorage, IStreamProvider
    {
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

        public Column LoadColumnMeta(string tablePath, string fileName, Registry registry)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.LoadColumnMeta(file, tablePath, registry);
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
