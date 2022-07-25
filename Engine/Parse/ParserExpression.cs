using System;
using System.Collections.Generic;

namespace Engine
{
    public static class ParserExpression
    {
        public static ExpressionNode Convert(string[] tokens, ref int pos)
        {
            if (pos >= tokens.Length)
                throw new Exception("Unexpected end of query");

            if (pos + 3 < tokens.Length &&
                tokens[pos].ToUpperInvariant() == "COUNT" &&
                tokens[pos + 1] == "(" &&
                tokens[pos + 3] == ")")
            {
                var result = new FunctionNode("COUNT", tokens[pos + 2]);
                pos += 4;
                return result;
            }

            return new ValueNode(tokens[pos++]);
        }
    }
}
