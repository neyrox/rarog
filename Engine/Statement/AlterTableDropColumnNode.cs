namespace Engine
{
    public class AlterTableDropColumnNode: Node
    {
        public string TableName;
        public string ColumnName;

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
