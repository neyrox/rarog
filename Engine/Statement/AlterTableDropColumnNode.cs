namespace Engine
{
    public class AlterTableDropColumnNode: Node
    {
        public readonly string TableName;
        public readonly string ColumnName;

        public override bool NeedWriterLock => true;

        public AlterTableDropColumnNode(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }

        public override Result Execute(Database db)
        {
            var table = db.GetTable(TableName);
            table.DropColumn(ColumnName);

            return Result.OK;
        }
    }
}
