using System.Collections.Generic;

namespace Engine
{
    public class DeleteNode: Node
    {
        public string TableName;
        public ConditionNode Condition;

        public DeleteNode(string tableName, ConditionNode condition)
        {
            TableName = tableName;
            Condition = condition;
        }

        public override Result Execute(Database db)
        {
            if (!db.ContainsTable(TableName))
                return Result.TableNotFound(TableName);

            db.GetTable(TableName).Delete(Condition);

            return Result.OK;
        }
    }
}
