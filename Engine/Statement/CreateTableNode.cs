using System.Collections.Generic;
using System.Threading;

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
            Table table;
            if (!Monitor.TryEnter(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                if (IfNotExists && db.ContainsTable(TableName))
                    return Result.OK;

                table = db.CreateTable(TableName);
            }
            finally
            {
                Monitor.Exit(db.SyncObject);
            }

            if (!Monitor.TryEnter(table.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockTable(TableName);

            try
            {
                for (int i = 0; i < ColumnNames.Count; ++i)
                    table.AddColumn(ColumnNames[i], DataTypes[i], Lengths[i]);
            }
            finally
            {
                Monitor.Exit(table.SyncObject);
            }

            return Result.OK;
        }
    }
}
