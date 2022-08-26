namespace Engine
{
    public class AlterTableDropColumnNode: BaseTableNode
    {
        public readonly string ColumnName;

        public AlterTableDropColumnNode(string tableName, string columnName)
            : base(tableName)
        {
            ColumnName = columnName;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            table.DropColumn(ColumnName);
            return Result.OK;
        }
    }
}
