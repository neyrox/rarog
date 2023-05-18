using System;
using System.Threading;

namespace Engine
{
    public abstract class Node
    {
        protected readonly TimeSpan LockTimeout = new TimeSpan(0, 1, 0);

        public abstract Result Execute(Database db, ref Transaction tx);

        protected Table GetTable(Database db, string tableName)
        {
            if (!Monitor.TryEnter(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                if (!db.ContainsTable(tableName))
                    throw Exceptions.TableNotFound(tableName);

                return db.GetTable(tableName);
            }
            finally
            {
                Monitor.Exit(db.SyncObject);
            }
        }
    }
}
