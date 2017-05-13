using System.Collections.Generic;

namespace Engine
{
    public class UpdateNode: Node
    {
        public string TableName;
        public List<string> ColumnNames;
        public List<string> Values;
        public List<ConditionNode> Conditions;

        public UpdateNode(string tableName, List<string> columnNames, List<string> values, List<ConditionNode> conditions)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Values = values;
            Conditions = conditions;
        }
    }
}
