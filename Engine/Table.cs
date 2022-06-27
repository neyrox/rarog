using System;
using System.Collections.Generic;

namespace Engine
{
    public class Table
    {
        private readonly string tableName;
        private readonly Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private int nextIdx = 0;

        public Table(string name)
        {
            tableName = name;
        }

        public Column GetColumn(string name)
        {
            return columns[name];
        }

        public void AddColumn(string name, string type, int length)
        {
            switch (type.ToLowerInvariant())
            {
                case "int":
                    AddColumn(name, new ColumnInteger(name));
                    break;
                case "float":
                case "double":
                    AddColumn(name, new ColumnDouble(name));
                    break;
                case "varchar":
                    AddColumn(name, new ColumnVarChar(name, length));
                    break;
                default:
                    throw new Exception($"Unknown type {type}");
            }
        }

        public void DropColumn(string name)
        {
            if (columns.ContainsKey(name))
                columns.Remove(name);
            else
                throw new Exception($"Column '{name}' not found in table '{tableName}'");
        }

        public void Update(List<string> columnNames, List<string> values, ConditionNode condition)
        {
            var rowsToUpdate = condition.GetRowsThatSatisfy(this);

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                for (int j = 0; j < rowsToUpdate.Count; ++j)
                {
                    int row = rowsToUpdate[j];
                    columns[columnName].Update(row, value);
                }
            }
        }

        public void Insert(List<string> columnNames, List<string> values)
        {
            var allColumnNames = new HashSet<string>(columns.Keys);
            var insertingColumnNames = new HashSet<string>(columnNames);

            foreach (var columnName in allColumnNames)
            {
                if (insertingColumnNames.Contains(columnName))
                    continue;

                columns[columnName].Insert(nextIdx, null);
            }

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                columns[columnName].Insert(nextIdx, value);
            }

            AddRow();
        }

        public List<ResultColumn> Select(List<string> columnNames, ConditionNode condition)
        {
            List<int> rowsToSelect = null;
            // TODO: replace with empty condition?
            if (condition != null)
            {
                rowsToSelect = condition.GetRowsThatSatisfy(this);
            }

            return Select(columnNames, rowsToSelect);
        }

        public void Delete(ConditionNode condition)
        {
            List<int> rowsToDelete = null;
            // TODO: replace with empty condition?
            if (condition != null)
            {
                rowsToDelete = condition.GetRowsThatSatisfy(this);
            }

            foreach (var column in columns)
            {
                column.Value.Delete(rowsToDelete);
            }
        }

        private void AddColumn(string name, Column column)
        {
            if (columns.Count > 0)
            {
                using (var enumerator = columns.GetEnumerator())
                {
                    enumerator.MoveNext();
                    var firstColumn = enumerator.Current.Value;
                    var indices = firstColumn.Indices;
                    foreach (var idx in indices)
                        column.Insert(idx, column.DefaultValue);
                }
            }

            columns.Add(name, column);
        }

        private List<ResultColumn> Select(List<string> columnNames, List<int> rows)
        {
            var result = new List<ResultColumn>();

            List<Column> columnsToQuery;
            if (columnNames[0] == "*")
            {
                columnsToQuery = new List<Column>(columns.Values);
            }
            else
            {
                columnsToQuery = new List<Column>();
                for (int i = 0; i < columnNames.Count; ++i)
                {
                    var columnName = columnNames[i];
                    columnsToQuery.Add(columns[columnName]);
                }
            }

            for (int j = 0; j < columnsToQuery.Count; ++j)
            {
                result.Add(columnsToQuery[j].Get(rows));
            }

            return result;
        }

        private void AddRow()
        {
            ++nextIdx;
        }
    }
}
