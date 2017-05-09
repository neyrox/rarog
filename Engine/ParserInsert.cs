using System.Collections.Generic;

namespace Engine
{
    public static class ParserInsert
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            var what = new List<string>();
            ++pos;  // skip "INSERT"
            if (ParserCommon.AssertUpperToken("INTO", tokens, pos))
                ++pos;  // skip "INTO"

            var tableName = string.Empty;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
                return null;

            var columnNames = new List<string>();
            var values = new List<string>();
            ConvertItems(tokens, columnNames, ref pos);

            if (ParserCommon.AssertToken("VALUES", tokens, pos))
                ++pos;
            else
                return null;

            ConvertItems(tokens, values, ref pos);

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            return new InsertNode(tableName, columnNames, values);
        }

        private static void ConvertItems(string[] tokens, List<string> items, ref int pos)
        {
            if (ParserCommon.AssertToken("(", tokens, pos))
                ++pos;
            else
                return;

            while (pos < tokens.Length)
            {
                if (ParserCommon.AssertToken(")", tokens, pos))
                {
                    ++pos;
                    break;
                }

                items.Add(tokens[pos]);  // TODO: check column names
                ++pos;
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken(",", tokens, pos))
                    ++pos;
            }
        }
    }
}
