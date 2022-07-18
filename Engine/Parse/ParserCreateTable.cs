using System;
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
                throw new Exception($"Unexpected token {tokens[pos]}");

            string tableName;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
            {
                throw new Exception("Unexpected end of query");
            }

            if (ParserCommon.AssertToken("(", tokens, pos))
                ++pos;
            else
                throw new Exception("Failed to find opening parenthesis at start of columns list");

            var columnNames = new List<string>();
            var dataTypes = new List<string>();
            var lengths = new List<int>();
            ConvertColumns(tokens, columnNames, dataTypes, lengths, ref pos);

            if (ParserCommon.AssertToken(")", tokens, pos))
                ++pos;
            else
                throw new Exception("Failed to find closing parenthesis at the end of columns list");

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                throw new Exception("Failed to find \';\' at the end of query");

            return new CreateTableNode(tableName, columnNames, dataTypes, lengths);
        }

        private static void ConvertColumns(string[] tokens, List<string> columnNames, List<string> dataTypes, List<int> lengths, ref int pos)
        {
            while (pos < tokens.Length)
            {
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
                        throw new Exception("Failed to find closing parenthesis at the field length");
                }
                else
                    lengths.Add(0);

                if (ParserCommon.AssertToken(",", tokens, pos))
                    ++pos;
                else
                    break;
            }
        }
    }
}
