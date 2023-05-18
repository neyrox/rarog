
using System;

namespace Engine
{
    public class Parser
    {
        public static Node Convert(string[] tokens)
        {
            Node root = null;
            int pos = 0;
            while (pos < tokens.Length)
            {
                var upperToken = tokens[pos].ToUpperInvariant();
                switch (upperToken)
                {
                    case "UPDATE":
                        root = ParserUpdate.Convert(tokens, ref pos);
                        break;
                    case "SELECT":
                        root = ParserSelect.Convert(tokens, ref pos);
                        break;
                    case "INSERT":
                        root = ParserInsert.Convert(tokens, ref pos);
                        break;
                    case "DELETE":
                        root = ParserDelete.Convert(tokens, ref pos);
                        break;
                    case "CREATE":
                        root = ParserCreateTable.Convert(tokens, ref pos);
                        break;
                    case "ALTER":
                        root = ParserAlterTable.Convert(tokens, ref pos);
                        break;
                    case "DROP":
                        root = ParserDropTable.Convert(tokens, ref pos);
                        break;
                    case "FLUSH":
                        root = ParserFlush.Convert(tokens, ref pos);
                        break;
                    case "BEGIN":
                        root = ParserBegin.Convert(tokens, ref pos);
                        break;
                    case "END":
                        root = ParserEnd.Convert(tokens, ref pos);
                        break;
                    default:
                        throw new Exception($"Unexpected token {tokens[pos]}");
                }
            }

            return root;
        }
    }
}
