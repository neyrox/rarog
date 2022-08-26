namespace Engine
{
    public class DeleteNode: BaseTableNode
    {
        public readonly ConditionNode Condition;

        public DeleteNode(string tableName, ConditionNode condition)
            : base(tableName)
        {
            Condition = condition;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            table.Delete(Condition);
            return Result.OK;
        }
    }
}
