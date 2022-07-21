using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public class FlushNode: Node
    {
        public override Result Execute(Database db)
        {
            if (!Monitor.TryEnter(db.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockDb();

            var syncObjects = new List<object> {db.SyncObject};
            try
            {
                foreach (var table in db.GetTables())
                {
                    if (!Monitor.TryEnter(table.SyncObject, LockTimeout))
                        throw Exceptions.FailedToLockTable(table.Name);
                    syncObjects.Add(table.SyncObject);
                }
                db.Flush();
            }
            finally
            {
                for (int i = syncObjects.Count - 1; i >= 0; --i)
                    Monitor.Exit(syncObjects[i]);
                syncObjects.Clear();
            }

            return Result.OK;
        }
    }
}
