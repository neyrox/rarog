using System.Collections.Generic;

namespace Engine
{
    // TODO: validate queries
    public class Database
    {
        private static List<List<string>> emptyResult = new List<List<string>>();
        private Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public Result Execute(UpdateNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return Result.TableNotFound(query.TableName);

            tables[query.TableName].Update(query.ColumnNames, query.Values, query.Conditions);

            return Result.OK;
        }

        public Result Execute(CreateTableNode spec)
        {
            var table = new Table();
            for (int i = 0; i < spec.ColumnNames.Count; ++i)
            {
                table.AddColumn(spec.ColumnNames[i], spec.DataTypes[i], spec.Lengths[i]);
            }

            tables.Add(spec.TableName, table);

            return Result.OK;
        }

        public Result Execute(DropTableNode query)
        {
            if (tables.Remove(query.TableName))
                return Result.OK;
            else
                return Result.TableNotFound(query.TableName);
        }

        public Result Execute(InsertNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return Result.TableNotFound(query.TableName);

            tables[query.TableName].Insert(query.ColumnNames, query.Values);

            return Result.OK;
        }

        public Result Execute(SelectNode query)
        {
            if (!tables.ContainsKey(query.TableName))
                return Result.TableNotFound(query.TableName);

            var rows = tables[query.TableName].Select(query.What, query.Conditions);
            return new Result(rows);
        }
    }
}
