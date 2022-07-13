using System.Collections.Generic;
using Engine.Storage;

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

        public override List<long> GetRowsThatSatisfy(Table table, IStorage storage)
        {
            var resultSet = new HashSet<long>(Left.GetRowsThatSatisfy(table, storage));
            var rightRows = Right.GetRowsThatSatisfy(table, storage);
            if (Operation == "AND")
            {
                resultSet.IntersectWith(rightRows);
            }
            else if (Operation == "OR")
            {
                resultSet.UnionWith(rightRows);
            }

            var result = new List<long>(resultSet);
            return result;
        }

    }
}
