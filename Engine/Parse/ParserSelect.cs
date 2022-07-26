using System;
using System.Collections.Generic;

namespace Engine
{
    public static class ParserSelect
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            var what = new List<ExpressionNode>();
            ++pos;  // skip "SELECT" itself
            ConvertWhat(tokens, what, ref pos);
            if (ParserCommon.AssertUpperToken("FROM", tokens, pos))
            {
                ++pos;
            }
            else if (ParserCommon.AssertToken(";", tokens, pos))
            {
                pos++;
                return new SelectWithoutTable(what);
            }
            else
            {
                throw new Exception("Unexpected end of query");
            }

            string tableName;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
            {
                throw new Exception("Expecting table name after WHERE");
            }

            ConditionNode condition;
            if (ParserCommon.AssertUpperToken("WHERE", tokens, pos))
            {
                ++pos;
                condition = ParserCondition.Convert(tokens, ref pos);
            }
            else
            {
                condition = new AnyConditionNode();
            }

            int limit = 0;
            if (ParserCommon.AssertUpperToken("LIMIT", tokens, pos))
            {
                ++pos;

                limit = int.Parse(tokens[pos++]);
                // forbid negative limits
                if (limit < 0)
                    return null;
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            var result = new SelectNode(what, tableName, condition, limit);
            return result;
        }

        private static void ConvertWhat(string[] tokens, List<ExpressionNode> what, ref int pos)
        {
            while (pos < tokens.Length)
            {
                if (tokens[pos] == ",")
                {
                    ++pos;
                    continue;
                }

                var tokenUpper = tokens[pos].ToUpperInvariant();
                if (tokenUpper == "FROM" || tokenUpper == ";")
                    break;

                what.Add(ParserExpression.Convert(tokens, ref pos));
            }
        }
    }
}
