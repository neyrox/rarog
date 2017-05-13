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

        public void Update(List<string> columnNames, List<string> values, List<ConditionNode> conditions)
        {
            var rowsToUpdate = GetRowsThatSatisfies(conditions);

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

        public List<List<string>> Select(List<string> columnNames, List<ConditionNode> conditions)
        {
            var rowsToSelect = GetRowsThatSatisfies(conditions);

            var result = new List<List<string>>();

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

            for (int i = 0; i < rowsToSelect.Count; ++i)
            {
                var rowNumber = rowsToSelect[i];
                var resultRow = new List<string>();
                for (int j = 0; j < columnsToQuery.Count; ++j)
                {
                    resultRow.Add(columnsToQuery[j].Get(rowNumber));
                }

                result.Add(resultRow);
            }

            return result;
        }

        private List<List<string>> Select(List<string> columnNames, List<int> rows)
        {
            var result = new List<List<string>>();

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

            for (int i = 0; i < rows.Count; ++i)
            {
                var row = new List<string>();
                for (int j = 0; j < columnsToQuery.Count; ++j)
                {
                    row.Add(columnsToQuery[j].Get(rows[i]));
                }

                result.Add(row);
            }

            return result;
        }


        private void AddRow()
        {
            allRows.Add(rowCount);
            ++rowCount;
        }

        private List<int> GetRowsThatSatisfies(List<ConditionNode> conditions)
        {
            if (conditions.Count == 0)
            {
                return allRows;
            }

            var resultRows = new List<int>();
            for (int i = 0; i < conditions.Count; ++i)
            {
                var condition = conditions[i];

                switch (condition.Operation)
                {
                    case "=":
                        var inclusions = columns[condition.ColumnName].GetInclusions(condition.Value);
                        resultRows.AddRange(inclusions);
                        break;
                    default:
                        break;
                }
            }

            return resultRows;
        }
    }
}
