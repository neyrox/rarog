using System.Collections.Generic;

namespace Engine
{
    public class UpdateNode: Node
    {
        public readonly string TableName;
        public readonly List<string> ColumnNames;
        public readonly List<string> Values;
        public readonly ConditionNode Condition;

        public override bool NeedWriterLock => true;

        public UpdateNode(string tableName, List<string> columnNames, List<string> values, ConditionNode condition)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Values = values;
            Condition = condition;
        }

        public override Result Execute(Database db)
        {
            if (!db.ContainsTable(TableName))
                return Result.TableNotFound(TableName);

            db.GetTable(TableName).Update(ColumnNames, Values, Condition);

            return Result.OK;
        }
    }
}
