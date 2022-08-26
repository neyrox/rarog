namespace Engine
{
    public class AlterTableAddColumnNode: BaseTableNode
    {
        public readonly string ColumnName;
        public readonly string DataType;
        public readonly int Length;

        public AlterTableAddColumnNode(string tableName, string columnName, string dataType, int length)
            : base(tableName)
        {
            ColumnName = columnName;
            DataType = dataType;
            Length = length;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            table.AddColumn(ColumnName, DataType, Length);
            return Result.OK;
        }
    }
}
