namespace Engine
{
    public class DeleteNode: BaseTableNode
    {
        public ConditionNode Condition;

        public DeleteNode(string tableName, ConditionNode condition)
            : base(tableName)
        {
            Condition = condition;
        }

        protected override Result ExecuteInternal(Table table)
        {
            table.Delete(Condition);
            return Result.OK;
        }
    }
}
