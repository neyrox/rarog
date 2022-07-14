namespace Engine
{
    public class AlterTableAddColumnNode: Node
    {
        public readonly string TableName;
        public readonly string ColumnName;
        public readonly string DataType;
        public readonly int Length;

        public override bool NeedWriterLock => true;

        public AlterTableAddColumnNode(string tableName, string columnName, string dataType, int length)
        {
            TableName = tableName;
            ColumnName = columnName;
            DataType = dataType;
            Length = length;
        }

        public override Result Execute(Database db)
        {
            var table = db.GetTable(TableName);
            table.AddColumn(ColumnName, DataType, Length);

            return Result.OK;
        }
    }
}
