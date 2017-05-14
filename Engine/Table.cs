using System.Collections.Generic;

namespace Engine
{
    public class Table
    {
        private Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private List<int> allRows = new List<int>();
        private int rowCount = 0;

        public void AddColumn(string name, string type, int length)
        {
            switch (type.ToLowerInvariant())
            {
                case "int":
                    columns.Add(name, new ColumnInteger());
                    break;
                default:
                    // TODO: log error
                    return;
            }
        }

        public void Update(List<string> columnNames, List<string> values, ConditionNode condition)
        {
            var rowsToUpdate = GetRowsThatSatisfies(condition);

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

                columns[columnName].Insert(null);
            }

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                columns[columnName].Insert(value);
            }

            AddRow();
        }

        public List<ResultColumnBase> Select(List<string> columnNames, ConditionNode condition)
        {
            var rowsToSelect = GetRowsThatSatisfies(condition);

            return Select(columnNames, rowsToSelect);
        }

        private List<ResultColumnBase> Select(List<string> columnNames, List<int> rows)
        {
            var result = new List<ResultColumnBase>();

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
            allRows.Add(rowCount);
            ++rowCount;
        }

        private List<int> GetRowsThatSatisfies(ConditionNode condition)
        {
            if (condition == null)
            {
                return allRows;
            }

            var resultRows = new List<int>();

            switch (condition.Operation)
            {
                case "=":
                    var inclusions = columns[condition.ColumnName].GetInclusions(condition.Value);
                    resultRows.AddRange(inclusions);
                    break;
                default:
                    break;
            }

            return resultRows;
        }
    }
}
