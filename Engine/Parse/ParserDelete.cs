using System.Collections.Generic;

namespace Engine
{
    public static class ParserDelete
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "DELETE" itself

            if (ParserCommon.AssertUpperToken("FROM", tokens, pos))
                ++pos;  // skip "FROM"
            else
                return null;

            var tableName = string.Empty;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
                return null;

            ConditionNode condition = null;
            if (ParserCommon.AssertUpperToken("WHERE", tokens, pos))
            {
                ++pos;

                condition = ParserCondition.Convert(tokens, ref pos);
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            var result = new DeleteNode(tableName, condition);
            return result;
        }
    }
}
