namespace Engine
{
    public class AlterTableDropColumnNode: BaseTableNode
    {
        public string ColumnName;

        public AlterTableDropColumnNode(string tableName, string columnName)
            : base(tableName)
        {
            ColumnName = columnName;
        }

        protected override Result ExecuteInternal(Table table)
        {
            table.DropColumn(ColumnName);
            return Result.OK;
        }
    }
}
