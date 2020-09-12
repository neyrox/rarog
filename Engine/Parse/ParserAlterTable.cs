using System.Collections.Generic;

namespace Engine
{
    public static class ParserAlterTable
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "ALTER"
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

            if (ParserCommon.AssertUpperToken("ADD", tokens, pos))
                ++pos;  // skip "ADD"
            else
                return null;

            var columnName = tokens[pos];  // TODO: check column names
            ++pos;
            if (pos >= tokens.Length)
                return null;

            var dataType = tokens[pos];  // TODO: check data types
            ++pos;
            if (pos >= tokens.Length)
                return null;

            int length;
            if (ParserCommon.AssertToken("(", tokens, pos))
            {
                ++pos;
                if (pos >= tokens.Length)
                    return null;
                length = int.Parse(tokens[pos]);
                ++pos;
                if (ParserCommon.AssertToken(")", tokens, pos))
                    ++pos;
                else
                    return null;
            }
            else
            {
                length = 0;
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            return new AlterTableAddColumnNode(tableName, columnName, dataType, length);
        }
    }
}
