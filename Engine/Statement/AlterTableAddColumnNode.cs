namespace Engine
{
    public class AlterTableAddColumnNode: Node
    {
        public string TableName;
        public string ColumnName;
        public string DataType;
        public int Length;

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
