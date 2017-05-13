using System.Collections.Generic;

namespace Engine
{
    public class InsertNode: Node
    {
        public string TableName;
        public List<string> ColumnNames;
        public List<string> Values;

        public InsertNode(string tableName, List<string> columnNames, List<string> values)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Values = values;
        }
    }
}
