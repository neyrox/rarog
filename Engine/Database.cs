using System.Collections.Generic;

namespace Engine
{
    public class Database
    {
        private Dictionary<string, Table> tables = new Dictionary<string, Table>();

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
            var table = new Table(tableName);
            tables.Add(tableName, table);
            return table;
        }

        public bool RemoveTable(string tableName)
        {
            return tables.Remove(tableName);
        }
    }
}
