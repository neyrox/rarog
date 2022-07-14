using System.Collections.Generic;

namespace Engine
{
    public class SelectNode: Node
    {
        public List<string> What;
        public string TableName;
        public ConditionNode Condition;
        public int Limit;

        public SelectNode(List<string> what, string tableName, ConditionNode condition, int limit)
        {
            What = what;
            TableName = tableName;
            Condition = condition;
            Limit = limit;
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
