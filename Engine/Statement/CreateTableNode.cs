using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public class CreateTableNode: Node
    {
        public readonly string TableName;
        public readonly List<string> ColumnNames;
        public readonly List<string> DataTypes;
        public readonly List<int> Lengths;
        public readonly bool IfNotExists;

        public CreateTableNode(string tableName, List<string> columnNames, List<string> dataTypes, List<int> lengths,
            bool ifNotExists)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            DataTypes = dataTypes;
            Lengths = lengths;
            IfNotExists = ifNotExists;
        }

        public override Result Execute(Database db, ref Transaction tx)
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

            if (!tx.TryLock(table.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockTable(TableName);

            for (int i = 0; i < ColumnNames.Count; ++i)
                table.AddColumn(ColumnNames[i], DataTypes[i], Lengths[i]);

            return Result.OK;
        }
    }
}
