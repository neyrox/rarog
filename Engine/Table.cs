using System.Collections.Generic;

namespace Engine
{
    public class Table
    {
        private Dictionary<string, Column> columns = new Dictionary<string, Column>();
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

            ++rowCount;
        }

        public List<List<string>> Select(List<string> columnNames)
        {
            var result = new List<List<string>>();

            List<Column> columnsToQuery;// = new List<Column>();
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

            for (int i = 0; i < rowCount; ++i)
            {
                var row = new List<string>();
                for (int j = 0; j < columnsToQuery.Count; ++j)
                {
                    row.Add(columnsToQuery[j].Get(i));
                }

                result.Add(row);
            }

            return result;
        }
    }
}
