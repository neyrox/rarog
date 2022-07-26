using System;
using System.Collections.Generic;

namespace Engine
{

    public abstract class ExpressionNode
    {
        public abstract void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result);
    }

    public class BinaryOperationNode: ExpressionNode
    {
        public readonly ExpressionNode Left;
        public readonly ExpressionNode Right;
        public readonly string Op;

        public BinaryOperationNode(ExpressionNode left, ExpressionNode right, string op)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public override void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result)
        {
            var leftResult = new List<ResultColumn>();
            var rightResult = new List<ResultColumn>();
            
            Left.Evaluate(table, rowsToSelect, leftResult);
            Right.Evaluate(table, rowsToSelect, rightResult);

            var v = new BinaryOpVisitor {Op = Op};
            for (int i = 0; i < leftResult.Count; ++i)
            {
                leftResult[i].Accept(v);
                rightResult[i].Accept(v);
                result.Add(v.ExtractResult());
            }
        }
    }

    public class ValueNode: ExpressionNode
    {
        public readonly string Item;

        public ValueNode(string item)
        {
            Item = item;
        }

        public override void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result)
        {
            if (table == null)
            {
                result.Add(Convert());
                return;
            }

            if (Item == "*")
            {
                foreach (var columnName in table.ColumnNames)
                    result.Add(table.GetColumn(columnName).Get(rowsToSelect));
            }
            else if (table.TryGetColumn(Item, out var column))
            {
                result.Add(column.Get(rowsToSelect));
            }
            else
            {
                result.Add(Convert());
            }
        }

        private ResultColumn Convert()
        {
            if (double.TryParse(Item, out var dblItem))
                return new ResultColumnDouble(string.Empty, new [] {dblItem});
            if (int.TryParse(Item, out var intItem))
                return new ResultColumnInteger(string.Empty, new [] {intItem});
            if (long.TryParse(Item, out var lngItem))
                return new ResultColumnBigInt(string.Empty, new [] {lngItem});

            return new ResultColumnString(string.Empty, new [] {Item});
        }
    }

    public class FunctionNode: ExpressionNode
    {
        public readonly string Function;
        public readonly ExpressionNode Item;

        public FunctionNode(string function, ExpressionNode item)
        {
            Function = function;
            Item = item;
        }

        public override void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result)
        {
            var ps = new List<ResultColumn>();
            Item.Evaluate(table, rowsToSelect, ps);

            switch (Function)
            {
                case "COUNT":
                    result.Add(new ResultColumnBigInt("COUNT", new long[] {ps.Count}));
                    break;
                default:
                    throw new Exception($"Unknown function {Function}");
            }
        }
    }
}
