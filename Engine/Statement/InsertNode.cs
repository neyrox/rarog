using System.Collections.Generic;

namespace Engine
{
    public class InsertNode: Node
    {
        public readonly string TableName;
        public readonly List<string> ColumnNames;
        public readonly List<string> Values;

        public override bool NeedWriterLock => true;

        public InsertNode(string tableName, List<string> columnNames, List<string> values)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Values = values;
        }

        public override Result Execute(Database db)
        {
            if (!db.ContainsTable(TableName))
                return Result.TableNotFound(TableName);

            db.GetTable(TableName).Insert(ColumnNames, Values);

            return Result.OK;
        }
    }
}
