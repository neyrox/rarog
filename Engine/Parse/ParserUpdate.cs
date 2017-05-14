using System.Collections.Generic;

namespace Engine
{
    public static class ParserUpdate
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "UPDATE"

            var tableName = string.Empty;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
                return null;

            if (ParserCommon.AssertToken("SET", tokens, pos))
                ++pos;
            else
                return null;

            var columnNames = new List<string>();
            var values = new List<string>();
            ConditionNode condition = null;
            ConvertColumns(tokens, columnNames, values, ref pos);

            if (ParserCommon.AssertToken("WHERE", tokens, pos))
            {
                ++pos;

                condition = ParserCondition.Convert(tokens, ref pos);
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            return new UpdateNode(tableName, columnNames, values, condition);
        }

        private static void ConvertColumns(string[] tokens, List<string> columnNames, List<string> values, ref int pos)
        {
            while (pos < tokens.Length)
            {
                columnNames.Add(tokens[pos]);  // TODO: check column names
                ++pos;
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken("=", tokens, pos))
                    ++pos;
                else
                    break;

                values.Add(tokens[pos]);
                ++pos;
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken(";", tokens, pos) || ParserCommon.AssertToken("WHERE", tokens, pos))
                    break;

                if (ParserCommon.AssertToken(",", tokens, pos))
                    ++pos;
                else
                    break;
            }
        }
    }
}
