using System.Collections.Generic;

namespace Engine
{
    public static class ParserCreateTable
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "CREATE"
            if (ParserCommon.AssertUpperToken("TABLE", tokens, pos))
                ++pos;  // skip "TABLE"
            else
                return null;

            string tableName;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
                return null;

            if (ParserCommon.AssertToken("(", tokens, pos))
                ++pos;
            else
                return null;

            var columnNames = new List<string>();
            var dataTypes = new List<string>();
            var lengths = new List<int>();
            ConvertColumns(tokens, columnNames, dataTypes, lengths, ref pos);

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            return new CreateTableNode(tableName, columnNames, dataTypes, lengths);
        }

        private static void ConvertColumns(string[] tokens, List<string> columnNames, List<string> dataTypes, List<int> lengths, ref int pos)
        {
            while (pos < tokens.Length)
            {
                if (ParserCommon.AssertToken(")", tokens, pos))
                {
                    ++pos;
                    break;
                }

                columnNames.Add(tokens[pos]);  // TODO: check column names
                ++pos;
                if (pos >= tokens.Length)
                    break;

                dataTypes.Add(tokens[pos]);  // TODO: check data types
                ++pos;
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken("(", tokens, pos))
                {
                    ++pos;
                    if (pos >= tokens.Length)
                        break;
                    lengths.Add(int.Parse(tokens[pos]));
                    ++pos;
                    if (ParserCommon.AssertToken(")", tokens, pos))
                        ++pos;
                    else
                        break;
                }
                else
                    lengths.Add(0);

                if (ParserCommon.AssertToken(",", tokens, pos))
                    ++pos;
            }
        }
    }
}
