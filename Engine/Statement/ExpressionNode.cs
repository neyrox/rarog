using System;
using System.Collections.Generic;

namespace Engine
{
    public abstract class ExpressionNode
    {
    }
    
    public class ValueNode: ExpressionNode
    {
        public string Item;
    }

    public class FunctionNode: ExpressionNode
    {
        public string Function;
        public string Item;
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

        public abstract ResultColumn Evaluate(Table table, List<long> rowsToSelect);
    }

    public class CountFunction : ExpressionFunction
    {
        public override ResultColumn Evaluate(Table table, List<long> rowsToSelect)
        {
            return new ResultColumnBigInt("COUNT", new long[] {rowsToSelect.Count});
        }
    }
}
