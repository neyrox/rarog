using System.Collections.Generic;

namespace Engine
{
    public class DeleteNode: Node
    {
        public string TableName;
        public ConditionNode Condition;

        public DeleteNode(string tableName, ConditionNode condition)
        {
            TableName = tableName;
            Condition = condition;
        }
    }
}
