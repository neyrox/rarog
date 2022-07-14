namespace Engine
{
    public class DropTableNode: Node
    {
        public readonly string TableName;

        public override bool NeedWriterLock => true;

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
