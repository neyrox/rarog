using System;
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
            if (!(expressionNode is ValueNode))
                throw new Exception("Only values supported for selects without table specification");

            var strValue = ((ValueNode) expressionNode).Item;

            if (int.TryParse(strValue, out var intValue))
                return new ResultColumnInteger(name, new[] {intValue});

            if (long.TryParse(strValue, out var longValue))
                return new ResultColumnBigInt(name, new[] {longValue});

            if (double.TryParse(strValue, out var doubleValue))
                return new ResultColumnDouble(name, new[] {doubleValue});

            return new ResultColumnString(name, new[] {strValue});
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
                if (expressionNode is ValueNode)
                    AddValues(table, rowsToSelect, (ValueNode)expressionNode, rows);
                else if (expressionNode is FunctionNode)
                    Evaluate(table, rowsToSelect, (FunctionNode)expressionNode, rows);
            }

            return new Result(rows);
        }

        private void AddValues(Table table, List<long> rowsToSelect, ValueNode valueNode, List<ResultColumn> results)
        {
            if (valueNode.Item != "*")
                results.Add(table.GetColumn(valueNode.Item).Get(rowsToSelect));

            foreach (var columnName in table.ColumnNames)
                results.Add(table.GetColumn(columnName).Get(rowsToSelect));
        }

        private void Evaluate(Table table, List<long> rowsToSelect, FunctionNode functionNode, List<ResultColumn> results)
        {
            var func = ExpressionFunction.Convert(functionNode);
            results.Add(func.Evaluate(table, rowsToSelect));
        }
    }
}
