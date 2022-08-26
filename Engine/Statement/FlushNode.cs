using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public class FlushNode: Node
    {
        public override Result Execute(Database db, ref Transaction tx)
        {
            if (!tx.TryLock(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            try
            {
                foreach (var table in db.GetTables())
                {
                    if (!tx.TryLock(table.SyncObject, LockTimeout))
                        throw Exceptions.FailedToLockTable(table.Name);
                }
                db.Flush();
            }
            finally
            {
                tx.Unlock();
            }

            return Result.OK;
        }
    }
}
