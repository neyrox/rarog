using System;
using System.Threading;

namespace Engine
{
    public class DropTableNode: Node
    {
        public string TableName;
        public bool IfExists;

        public DropTableNode(string tableName, bool ifExists)
        {
            TableName = tableName;
            IfExists = ifExists;
        }

        public override Result Execute(Database db)
        {
            if (!Monitor.TryEnter(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                if (db.RemoveTable(TableName))
                    return Result.OK;

                if (IfExists)
                    return Result.OK;
            }
            finally
            {
                Monitor.Exit(db.SyncObject);
            }

            throw Exceptions.TableNotFound(TableName);
        }
    }
}
