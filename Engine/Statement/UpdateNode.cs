using System.Collections.Generic;
using Engine.Statement;

namespace Engine
{
    public class UpdateNode: Node
    {
        public string TableName;
        public List<string> ColumnNames;
        public List<OperationNode> Ops;
        public ConditionNode Condition;

        public UpdateNode(string tableName, List<string> columnNames, List<OperationNode> ops, ConditionNode condition)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Ops = ops;
            Condition = condition;
        }

        public override Result Execute(Database db)
        {
            if (!db.ContainsTable(TableName))
                return Result.TableNotFound(TableName);

            db.GetTable(TableName).Update(ColumnNames, Ops, Condition);

            return Result.OK;
        }
    }
}
