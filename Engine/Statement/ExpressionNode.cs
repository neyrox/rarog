using System;
using System.Collections.Generic;
using System.Globalization;

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
                result.Add(Convert(1));
                return;
            }

            var count = rowsToSelect?.Count ?? 1;

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
                result.Add(Convert(count));
            }
        }

        private ResultColumn Convert(int count)
        {
            if (int.TryParse(Item, out var intItem))
                return new ResultColumnInteger(string.Empty, ArrayOf(intItem, count));
            if (long.TryParse(Item, out var lngItem))
                return new ResultColumnBigInt(string.Empty, ArrayOf(lngItem, count));
            if (double.TryParse(Item, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var dblItem))
                return new ResultColumnDouble(string.Empty, ArrayOf(dblItem, count));

            return new ResultColumnString(string.Empty, ArrayOf(Item, count));
        }

        private T[] ArrayOf<T>(T value, int count)
        {
            var result = new T[count];
            for (int i = 0; i < count; i++)
                result[i] = value;
            return result;
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
                    Count(ps, result);
                    break;
                case "MIN":
                    Min(ps, result);
                    break;
                case "MAX":
                    Max(ps, result);
                    break;
                default:
                    throw new Exception($"Unknown function {Function}");
            }
        }

        private void Count(List<ResultColumn> ps, List<ResultColumn> result)
        {
            result.Add(new ResultColumnBigInt("COUNT", new long[] {ps[0].Count}));
        }

        private void Min(List<ResultColumn> sources, List<ResultColumn> result)
        {
            for (int i = 0; i < sources.Count; ++i)
            {
                var visitor = new MinVisitor();
                sources[i].Accept(visitor);
                result.Add(visitor.ExtractResult());
            }
        }
        
        private void Max(List<ResultColumn> sources, List<ResultColumn> result)
        {
            for (int i = 0; i < sources.Count; ++i)
            {
                var visitor = new MaxVisitor();
                sources[i].Accept(visitor);
                result.Add(visitor.ExtractResult());
            }
        }
    }
}
