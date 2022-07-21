using System.Collections.Generic;

namespace Engine
{
    public class CreateTableNode: Node
    {
        public string TableName;
        public List<string> ColumnNames;
        public List<string> DataTypes;
        public List<int> Lengths;
        public bool IfNotExists;

        public CreateTableNode(string tableName, List<string> columnNames, List<string> dataTypes, List<int> lengths,
            bool ifNotExists)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            DataTypes = dataTypes;
            Lengths = lengths;
            IfNotExists = ifNotExists;
        }

        public override Result Execute(Database db)
        {
            if (IfNotExists && db.ContainsTable(TableName))
                return Result.OK;

            var table = db.CreateTable(TableName);
            for (int i = 0; i < ColumnNames.Count; ++i)
                table.AddColumn(ColumnNames[i], DataTypes[i], Lengths[i]);

            return Result.OK;
        }
    }
}
