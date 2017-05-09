using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: Node
    {
        public List<string> What;
        public string From;

        public SelectNode(List<string> what, string from)
        {
            What = what;
            From = from;
        }
    }
}
