using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: Node
    {
        public List<string> What;
        public string TableName;
        public ConditionNode Condition;

        public SelectNode(List<string> what, string tableName, ConditionNode condition)
        {
            What = what;
            TableName = tableName;
            Condition = condition;
        }
    }
}
