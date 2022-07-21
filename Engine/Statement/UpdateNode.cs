using System.Collections.Generic;
using Engine.Statement;

namespace Engine
{
    public class UpdateNode: BaseTableNode
    {
        public List<string> ColumnNames;
        public List<OperationNode> Ops;
        public ConditionNode Condition;

        public UpdateNode(string tableName, List<string> columnNames, List<OperationNode> ops, ConditionNode condition)
            : base(tableName)
        {
            ColumnNames = columnNames;
            Ops = ops;
            Condition = condition;
        }

        protected override Result ExecuteInternal(Table table)
        {
            table.Update(ColumnNames, Ops, Condition);
            return Result.OK;
        }
    }
}
