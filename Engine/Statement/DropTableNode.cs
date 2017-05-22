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

        public override Result Execute(Database db)
        {
            if (db.RemoveTable(TableName))
                return Result.OK;
            else
                return Result.TableNotFound(TableName);
        }
    }
}
