using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: Node
    {
        public List<string> What;
        public string TableName;
        public List<ConditionNode> Conditions;

        public SelectNode(List<string> what, string tableName, List<ConditionNode> conditions)
        {
            What = what;
            TableName = tableName;
            Conditions = conditions;
        }
    }
}
