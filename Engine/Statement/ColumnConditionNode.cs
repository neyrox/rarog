using System.Collections.Generic;
using Engine.Storage;

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

        public override List<long> GetRowsThatSatisfy(Table table, IStorage storage)
        {
            return table.GetColumn(ColumnName).Filter(Operation, Value, storage);
        }
    }
}
