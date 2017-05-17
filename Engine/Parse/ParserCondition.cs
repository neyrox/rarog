using System.Collections.Generic;

namespace Engine
{
    public static class ParserCondition
    {
        public static ConditionNode Convert(string[] tokens, ref int pos)
        {
            ConditionNode result = ConvertSingle(tokens, ref pos);
            while (!ParserCommon.AssertToken(";", tokens, pos))
            {
                if (ParserCommon.AssertUpperToken("AND", tokens, pos))
                {
                    ++pos;
                    var node2 = ConvertSingle(tokens, ref pos);
                    result = new CompositeConditionNode(result, "AND", node2);
                }
                else if (ParserCommon.AssertUpperToken("OR", tokens, pos))
                {
                    ++pos;
                    var node2 = ConvertSingle(tokens, ref pos);
                    result = new CompositeConditionNode(result, "OR", node2);
                }

            }
            return result;
        }

        public static ConditionNode ConvertSingle(string[] tokens, ref int pos)
        {
            var columnName = string.Empty;
            if (pos < tokens.Length)
            {
                columnName = tokens[pos];
                ++pos;
            }
            else
                return null;

            var operation = string.Empty;
            if (pos < tokens.Length)
            {
                operation = tokens[pos];
                ++pos;
            }
            else
                return null;

            var value = string.Empty;
            if (pos < tokens.Length)
            {
                value = tokens[pos];
                ++pos;
            }
            else
                return null;

            return new ColumnConditionNode(columnName, operation, value);
        }
    }
}
