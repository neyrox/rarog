using System.Collections.Generic;

namespace Engine
{
    public class CreateTableNode: Node
    {
        public readonly string TableName;
        public readonly List<string> ColumnNames;
        public readonly List<string> DataTypes;
        public readonly List<int> Lengths;

        public override bool NeedWriterLock => true;

        public CreateTableNode(string tableName, List<string> columnNames, List<string> dataTypes, List<int> lengths)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            DataTypes = dataTypes;
            Lengths = lengths;
        }

        public override Result Execute(Database db)
        {
            var table = db.CreateTable(TableName);
            for (int i = 0; i < ColumnNames.Count; ++i)
            {
                table.AddColumn(ColumnNames[i], DataTypes[i], Lengths[i]);
            }

            return Result.OK;
        }
    }
}
