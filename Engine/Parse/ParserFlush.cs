using System;

namespace Engine
{
    public static class ParserFlush
    {
        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "FLUSH"

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                throw new Exception("Failed to find \';\' at the end of query");

            return new FlushNode();
        }
    }
}
