using System.Collections.Generic;

namespace Engine
{
    public class DropTableNode: Node
    {
        public string TableName;

        public DropTableNode(string tableName)
        {
            TableName = tableName;
        }
    }
}
