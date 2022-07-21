using System.Collections.Generic;

namespace Engine
{
    public class DropTableNode: Node
    {
        public string TableName;
        public bool IfExists;

        public DropTableNode(string tableName, bool ifExists)
        {
            TableName = tableName;
            IfExists = ifExists;
        }

        public override Result Execute(Database db)
        {
            if (db.RemoveTable(TableName))
                return Result.OK;

            if (IfExists)
                return Result.OK;

            return Result.TableNotFound(TableName);
        }
    }
}
