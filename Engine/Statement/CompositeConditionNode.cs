using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class CompositeConditionNode : ConditionNode
    {
        public readonly ConditionNode Left;
        public readonly ConditionNode Right;
        private readonly string Operation;

        public CompositeConditionNode(ConditionNode left, string operation, ConditionNode right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public override List<long> GetRowsThatSatisfy(Table table, int limit)
        {
            HashSet<long> resultSet;
            if (Operation == "AND")
            {
                // TODO: optimize with streaming
                resultSet = new HashSet<long>(Left.GetRowsThatSatisfy(table, 0));
                var rightRows = Right.GetRowsThatSatisfy(table, 0);
                resultSet.IntersectWith(rightRows);
            }
            else if (Operation == "OR")
            {
                resultSet = new HashSet<long>(Left.GetRowsThatSatisfy(table, limit));
                var rightRows = Right.GetRowsThatSatisfy(table, limit);
                resultSet.UnionWith(rightRows);
            }
            else
            {
                throw new Exception($"Unknown operation \'{Operation}\'");
            }

            var result = new List<long>(limit > 0 ? resultSet.Take(limit) : resultSet);
            return result;
        }
    }
}
