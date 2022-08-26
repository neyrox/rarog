using System.Collections.Generic;

namespace Engine
{
    public static class ParserInsert
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "INSERT"
            if (ParserCommon.AssertUpperToken("INTO", tokens, pos))
                ++pos;  // skip "INTO"

            string tableName;
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

            var capacitor = string.Empty;

            while (pos < tokens.Length)
            {
                if (ParserCommon.AssertToken(")", tokens, pos))
                {
                    items.Add(capacitor);
                    ++pos;
                    break;
                }

                capacitor += tokens[pos++];
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken(",", tokens, pos))
                {
                    items.Add(capacitor);
                    capacitor = string.Empty;
                    ++pos;
                }
            }
        }
    }
}
