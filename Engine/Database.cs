using System.Collections.Generic;

namespace Engine
{
    public class Database
    {
        private static List<List<string>> emptyResult = new List<List<string>>();
        private Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public List<List<string>> Execute(CreateTableNode spec)
        {
            var table = new Table();
            for (int i = 0; i < spec.ColumnNames.Count; ++i)
            {
                table.AddColumn(spec.ColumnNames[i], spec.DataTypes[i], spec.Lengths[i]);
            }

            tables.Add(spec.TableName, table);

            return emptyResult;
        }

        public List<List<string>> Execute(InsertNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return emptyResult;

            tables[query.TableName].Insert(query.ColumnNames, query.Values);

            return emptyResult;
        }

        public List<List<string>> Execute(SelectNode query)
        {
            if (!tables.ContainsKey(query.From))
                return emptyResult;

            return tables[query.From].Select(query.What);
        }
    }
}
