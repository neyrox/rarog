using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Storage
{
    public class FileStorage : IStorage
    {
        public Column LoadColumn(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return StreamStorage.LoadColumn(file);
            }
        }

        public void Store(ColumnInteger column, string tableDir)
        {
            // TODO: file versioning
            using (var file = File.Open(Column.GetFileName(tableDir, column.Name), FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamStorage.Store(column, file);
            }
        }

        public void Store(ColumnDouble column, string tableDir)
        {
            // TODO: file versioning
            using (var file = File.Open(Column.GetFileName(tableDir, column.Name), FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamStorage.Store(column, file);
            }
        }

        public void Store(ColumnVarChar column, string tableDir)
        {
            // TODO: file versioning
            using (var file = File.Open(Column.GetFileName(tableDir, column.Name), FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamStorage.Store(column, file);
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
                if (!columnFile.EndsWith(Column.ColumnFileExtension))
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