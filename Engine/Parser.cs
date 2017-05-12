
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
                    case "CREATE":
                        root = ParserCreateTable.Convert(tokens, ref pos);
                        break;
                    default:
                        // TODO: log unknown token
                        return null;
                }
            }

            return root;
        }

    }
}
