using System.Collections.Generic;

namespace Engine
{
    public class SelectWithoutTable: Node
    {
        public readonly List<ExpressionNode> What;

        public SelectWithoutTable(List<ExpressionNode> what)
        {
            What = what;
        }

        public override Result Execute(Database db, ref Transaction tx)
        {
            var rows = new List<ResultColumn>();
            for (int i = 0; i < What.Count; ++i)
            {
                var expression = What[i];
                rows.Add(Convert(i.ToString(), expression));
            }
            return new Result(rows);
        }

        private ResultColumn Convert(string name, ExpressionNode expressionNode)
        {
            var res = new List<ResultColumn>();
            expressionNode.Evaluate(null, null, res);
            return res[0];
        }
    }

    public class SelectNode: BaseTableNode
    {
        public readonly List<ExpressionNode> What;
        public readonly ConditionNode Condition;
        public readonly int Limit;

        public SelectNode(List<ExpressionNode> what, string tableName, ConditionNode condition, int limit)
            : base(tableName)
        {
            What = what;
            Condition = condition;
            Limit = limit;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            // TODO: optimize for empty condition etc
            var rowsToSelect = Condition.GetRowsThatSatisfy(table, Limit);

            var rows = new List<ResultColumn>();

            for (int i = 0; i < What.Count; ++i)
            {
                var expressionNode = What[i];
                expressionNode.Evaluate(table, rowsToSelect, rows);
            }

            return new Result(rows);
        }
    }
}
