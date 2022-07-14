using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: Node
    {
        public readonly List<string> What;
        public readonly string TableName;
        public readonly ConditionNode Condition;

        public override bool NeedWriterLock => false;

        public SelectNode(List<string> what, string tableName, ConditionNode condition)
        {
            What = what;
            TableName = tableName;
            Condition = condition;
        }

        public override Result Execute(Database db)
        {
            if (!db.ContainsTable(TableName))
                return Result.TableNotFound(TableName);

            var rows = db.GetTable(TableName).Select(What, Condition);
            return new Result(rows);
        }
    }
}
