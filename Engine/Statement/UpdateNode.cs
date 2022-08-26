using System.Collections.Generic;
using Engine.Statement;

namespace Engine
{
    public class UpdateNode: BaseTableNode
    {
        public readonly List<string> ColumnNames;
        public readonly List<OperationNode> Ops;
        public readonly ConditionNode Condition;

        public UpdateNode(string tableName, List<string> columnNames, List<OperationNode> ops, ConditionNode condition)
            : base(tableName)
        {
            ColumnNames = columnNames;
            Ops = ops;
            Condition = condition;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            table.Update(ColumnNames, Ops, Condition);
            return Result.OK;
        }
    }
}
