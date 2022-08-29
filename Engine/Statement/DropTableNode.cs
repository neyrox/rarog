using System.Threading;

namespace Engine
{
    public class DropTableNode: Node
    {
        public readonly string TableName;
        public readonly bool IfExists;

        public DropTableNode(string tableName, bool ifExists)
        {
            TableName = tableName;
            IfExists = ifExists;
        }

        public override Result Execute(Database db, ref Transaction tx)
        {
            if (!Monitor.TryEnter(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                if (db.ContainsTable(TableName))
                {
                    var table = GetTable(db, TableName);
                    if (!tx.TryLock(table.Sem, LockTimeout))
                        throw Exceptions.FailedToLockTable(TableName);

                    if (db.RemoveTable(TableName))
                        return Result.OK;
                }
                else if (!IfExists)
                {
                    throw Exceptions.TableNotFound(TableName);
                }
            }
            finally
            {
                Monitor.Exit(db.SyncObject);
            }

            return Result.OK;
        }
    }
}
