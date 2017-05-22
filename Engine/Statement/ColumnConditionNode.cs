using System.Collections.Generic;

namespace Engine
{
    public class ColumnConditionNode: ConditionNode
    {
        public string ColumnName;
        public string Operation;
        public string Value;

        public ColumnConditionNode(string columnName, string operation, string value)
        {
            ColumnName = columnName;
            Operation = operation;
            Value = value;
        }

        public override List<int> GetRowsThatSatisfy(Table table)
        {
            return table.GetColumn(ColumnName).Filter(Operation, Value);
        }
    }
}
