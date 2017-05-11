using System.Collections.Generic;

namespace Engine
{
    public static class ParserCondition
    {
        public static ConditionNode Convert(string[] tokens, ref int pos)
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

            return new ConditionNode(columnName, operation, value);
        }
    }
}
