
namespace Engine
{
    public class ConditionNode
    {
        public string ColumnName;
        public string Operation;
        public string Value;

        public ConditionNode(string columnName, string operation, string value)
        {
            ColumnName = columnName;
            Operation = operation;
            Value = value;
        }
    }
}
