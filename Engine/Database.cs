using System.Collections.Generic;

namespace Engine
{
    // TODO: validate queries
    public class Database
    {
        private static List<List<string>> emptyResult = new List<List<string>>();
        private Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public void Execute(UpdateNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return;

            tables[query.TableName].Update(query.ColumnNames, query.Values, query.Conditions);

            return;
        }

        public void Execute(CreateTableNode spec)
        {
            var table = new Table();
            for (int i = 0; i < spec.ColumnNames.Count; ++i)
            {
                table.AddColumn(spec.ColumnNames[i], spec.DataTypes[i], spec.Lengths[i]);
            }

            tables.Add(spec.TableName, table);

            return;
        }

        public void Execute(InsertNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return;

            tables[query.TableName].Insert(query.ColumnNames, query.Values);
        }

        public List<List<string>> Execute(SelectNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return emptyResult;

            return tables[query.TableName].Select(query.What, query.Conditions);
        }
    }
}
