using System.Collections.Generic;

namespace Engine
{
    public class UpdateNode: Node
    {
        public string TableName;
        public List<string> ColumnNames;
        public List<string> Values;
        public ConditionNode Condition;

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
