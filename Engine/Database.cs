using System;
using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public class Database
    {
        public readonly object SyncObject = new object();
        private readonly SortedList<string, Table> tables = new SortedList<string, Table>();
        private readonly IStorage storage;

        private readonly Registry registry;

        public Database(IStorage storage)
        {
            this.storage = storage;
            registry = new Registry(storage);
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
            var table = new Table(tableName, storage, registry);
            table.Store();
            tables.Add(tableName, table);
            return table;
        }

        public bool RemoveTable(string tableName)
        {
            var result = tables.TryGetValue(tableName, out var table);
            if (!result)
                return false;

            tables.Remove(tableName);

            table.Drop();

            return true;
        }

        public void Load()
        {
            foreach (var tableName in storage.GetTableNames())
            {
                Console.WriteLine($"Loading table {tableName}");
                var table = new Table(tableName, storage, registry);
                table.Load();
                tables.Add(table.Name, table);
            }
        }

        public IEnumerable<Table> GetTables()
        {
            return tables.Values;
        }

        public void Flush()
        {
            registry.IntTraits.PageStorage.Flush();
            registry.BigIntTraits.PageStorage.Flush();
            registry.DoubleTraits.PageStorage.Flush();
            registry.StrTraits.PageStorage.Flush();
        }
    }
}
