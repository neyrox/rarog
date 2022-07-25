using System;
using System.Collections.Generic;

namespace Engine
{
    public abstract class ExpressionNode
    {
    }
    
    public class ValueNode: ExpressionNode
    {
        public readonly string Item;

        public ValueNode(string item)
        {
            Item = item;
        }
    }

    public class FunctionNode: ExpressionNode
    {
        public readonly string Function;
        public readonly string Item;

        public FunctionNode(string function, string item)
        {
            Function = function;
            Item = item;
        }
    }

    public abstract class ExpressionFunction
    {
        public static ExpressionFunction Convert(FunctionNode functionNode)
        {
            switch (functionNode.Function)
            {
                case "COUNT":
                    if (functionNode.Item == "*")
                        return new CountFunction();
                    else
                        throw new Exception("Column-specific COUNT are not yet implemented");
                default:
                    throw new Exception($"Unknown function {functionNode.Function}");
            }
        }

        public abstract void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result);
    }

    public class CountFunction : ExpressionFunction
    {
        public override void Evaluate(Table table, List<long> rowsToSelect, List<ResultColumn> result)
        {
            result.Add(new ResultColumnBigInt("COUNT", new long[] {rowsToSelect.Count}));
        }
    }
}
