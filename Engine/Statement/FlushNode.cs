using System;
using System.Threading;

namespace Engine
{
    public class FlushNode: Node
    {
        private readonly TimeSpan FlushLockTimeout = new TimeSpan(0, 0, 1);

        public override Result Execute(Database db, ref Transaction tx)
        {
            if (!Monitor.TryEnter(db.SyncObject, FlushLockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                foreach (var table in db.GetTables())
                {
                    if (!tx.TryLock(table.Sem, FlushLockTimeout))
                        throw Exceptions.FailedToLockTable(table.Name);
                }
                db.Flush();
            }
            finally
            {
                tx.Unlock();
                Monitor.Exit(db.SyncObject);
            }

            return Result.OK;
        }
    }
}
