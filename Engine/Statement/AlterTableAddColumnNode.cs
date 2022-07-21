namespace Engine
{
    public class AlterTableAddColumnNode: BaseTableNode
    {
        public string ColumnName;
        public string DataType;
        public int Length;

        public AlterTableAddColumnNode(string tableName, string columnName, string dataType, int length)
            : base(tableName)
        {
            ColumnName = columnName;
            DataType = dataType;
            Length = length;
        }

        protected override Result ExecuteInternal(Table table)
        {
            table.AddColumn(ColumnName, DataType, Length);
            return Result.OK;
        }
    }
}
