using System.Collections.Generic;

namespace Engine
{
    public class SelectWithoutTable: Node
    {
        public List<ExpressionNode> What;

        public SelectWithoutTable(List<ExpressionNode> what)
        {
            What = what;
        }

        public override Result Execute(Database db)
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
        public List<ExpressionNode> What;
        public ConditionNode Condition;
        public int Limit;

        public SelectNode(List<ExpressionNode> what, string tableName, ConditionNode condition, int limit)
            : base(tableName)
        {
            What = what;
            Condition = condition;
            Limit = limit;
        }

        protected override Result ExecuteInternal(Table table)
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
