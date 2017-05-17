using System.Collections.Generic;

namespace Engine
{
    public class CompositeConditionNode : ConditionNode
    {
        public ConditionNode Left;
        public ConditionNode Right;
        public string Operation;

        public CompositeConditionNode(ConditionNode left, string operation, ConditionNode right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public override List<int> GetRowsThatSatisfy(Table table)
        {
            var resultSet = new HashSet<int>(Left.GetRowsThatSatisfy(table));
            var rightRows = Right.GetRowsThatSatisfy(table);
            if (Operation == "AND")
            {
                //resultSet.UnionWith(rightRows);
                resultSet.IntersectWith(rightRows);
            }
            else if (Operation == "OR")
            {
                resultSet.UnionWith(rightRows);
            }

            List<int> result = new List<int>(resultSet);
            return result;
        }

    }
}
