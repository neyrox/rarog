using System;
using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public class Database
    {
        private readonly Dictionary<string, Table> tables = new Dictionary<string, Table>();
        private readonly IStorage storage;

        public Database(IStorage storage)
        {
            this.storage = storage;
        }

        public bool ContainsTable(string tableName)
        {
            return tables.ContainsKey(tableName);
        }

        public Table GetTable(string tableName)
        {
            return tables[tableName];
        }

        public Table CreateTable(string tableName)
        {
            var table = new Table(tableName, storage);
            table.Store();
            tables.Add(tableName, table);
            return table;
        }

        public bool RemoveTable(string tableName)
        {
            var result = tables.TryGetValue(tableName, out var table);
            if (!result)
                return false;

            var tableDir = table.GetTableDir();
            var columnFiles = storage.GetColumnFiles(tableDir);
            foreach (var columnFile in columnFiles)
            {
                if (!columnFile.EndsWith(Column.ColumnFileExtension))
                    continue;
                
                storage.DeleteFile(columnFile);
            }

            return true;
        }

        public void Load()
        {
            foreach (var tableName in storage.GetTableNames())
            {
                Console.WriteLine($"Loading table {tableName}");
                var table = new Table(tableName, storage);
                table.Load();
                tables.Add(table.Name, table);
            }
        }
    }
}
