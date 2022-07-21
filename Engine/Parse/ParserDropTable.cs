using System;

namespace Engine
{
    public static class ParserDropTable
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "DROP"
            if (ParserCommon.AssertUpperToken("TABLE", tokens, pos))
                ++pos;  // skip "TABLE"
            else
                throw new Exception($"Unexpected token {tokens[pos]}");

            bool ifExists = false;
            if (ParserCommon.AssertUpperToken("IF", tokens, pos) &&
                ParserCommon.AssertUpperToken("EXISTS", tokens, pos + 1))
            {
                ifExists = true;
                pos += 2;
            }

            string tableName;
            if (pos < tokens.Length)
                tableName = tokens[pos++];
            else
                throw new Exception("Unexpected end of query");

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                throw new Exception("Failed to find \';\' at the end of query");

            return new DropTableNode(tableName, ifExists);
        }
    }
}
