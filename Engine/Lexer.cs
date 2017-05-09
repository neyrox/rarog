using System.Collections.Generic;

namespace Engine
{
    public class Lexer
    {
        private static string whiteSpace = " \t\r\n";
        private static string singleTokens = "*,;()=";
        private static string symbolStoppers = singleTokens + whiteSpace;
        private static string quotes = "\'\"";

        public static string[] Split(string query)
        {
            int pos = 0;
            var buffer = new List<string>();

            while (pos < query.Length)
            {
                var cur = query[pos];
                if (whiteSpace.IndexOf(cur) > -1)
                {
                    pos++;
                }
                else if (singleTokens.IndexOf(cur) > -1)
                {
                    // TODO: get token from pool
                    buffer.Add(new string(cur, 1));
                    pos++;
                }
                else if (quotes.IndexOf(cur) > -1)
                {
                    StringToken(query, buffer, ref pos);
                }
                else
                {
                    SymbolToken(query, buffer, ref pos);
                }
            }

            return buffer.ToArray();
        }

        public static void SymbolToken(string query, List<string> buffer, ref int pos)
        {
            int start = pos;
            while (pos < query.Length)
            {
                if (symbolStoppers.IndexOf(query[pos]) == -1)
                {
                    pos++;
                }
                else
                {
                    break;
                }
            }

            // TODO: get token from pool, if present
            buffer.Add(query.Substring(start, pos - start));
        }

        public static void StringToken(string query, List<string> buffer, ref int pos)
        {
            int start = pos++;
            while (pos < query.Length)
            {
                if (quotes.IndexOf(query[pos]) > -1)
                {
                    pos++;
                    break;
                }
                pos++;
            }

            // TODO: get token from pool, if present
            buffer.Add(query.Substring(start+1, pos-start-2));
        }
    }
}
