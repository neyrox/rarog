using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: BaseTableNode
    {
        public List<string> What;
        public ConditionNode Condition;
        public int Limit;

        public SelectNode(List<string> what, string tableName, ConditionNode condition, int limit)
            : base(tableName)
        {
            What = what;
            Condition = condition;
            Limit = limit;
        }

        protected override Result ExecuteInternal(Table table)
        {
            var rows = table.Select(What, Condition, Limit);
            return new Result(rows);
        }
    }
}
