using System.Collections.Generic;

namespace Engine
{
    public static class ParserSelect
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            var what = new List<string>();
            ++pos;  // skip "SELECT" itself
            ConvertWhat(tokens, what, ref pos);
            if (ParserCommon.AssertUpperToken("FROM", tokens, pos))
                ++pos;
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

            var conditions = new List<ConditionNode>();
            if (ParserCommon.AssertToken("WHERE", tokens, pos))
            {
                ++pos;

                var condition = ParserCondition.Convert(tokens, ref pos);
                conditions.Add(condition);
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            var result = new SelectNode(what, tableName, conditions);
            return result;
        }

        private static void ConvertWhat(string[] tokens, List<string> what, ref int pos)
        {
            while (pos < tokens.Length)
            {
                if (tokens[pos] != ",")
                {
                    var tokenUpper = tokens[pos].ToUpperInvariant();
                    if (tokenUpper == "FROM")
                        break;
                    what.Add(tokens[pos]);
                }
                ++pos;
            }
        }
    }
}
