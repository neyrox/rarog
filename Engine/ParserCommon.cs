
namespace Engine
{
    public static class ParserCommon
    {
        public static bool AssertUpperToken(string expected, string[] tokens, int pos)
        {
            return pos < tokens.Length && tokens[pos].ToUpperInvariant() == expected;
        }
        public static bool AssertToken(string expected, string[] tokens, int pos)
        {
            return pos < tokens.Length && tokens[pos] == expected;
        }
    }
}
