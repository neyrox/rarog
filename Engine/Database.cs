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

        public Database(IStorage storage)
        {
            this.storage = storage;
        }

        public bool ContainsTable(string tableName)
        {
            lock (SyncObject)
            {
                return tables.ContainsKey(tableName);
            }
        }

        public Table GetTable(string tableName)
        {
            lock (SyncObject)
            {
                return tables[tableName];
            }
        }

        public Table CreateTable(string tableName)
        {
            lock (SyncObject)
            {
                var table = new Table(tableName, storage);
                table.Store();
                tables.Add(tableName, table);
                return table;
            }
        }

        public bool RemoveTable(string tableName)
        {
            lock (SyncObject)
            {
                var result = tables.TryGetValue(tableName, out var table);
                if (!result)
                    return false;

                tables.Remove(tableName);

                table.DeleteSelf();

                return true;
            }
        }

        public void Load()
        {
            lock (SyncObject)
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

        public void Flush()
        {
            try
            {
                lock (SyncObject)
                {
                    storage.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
