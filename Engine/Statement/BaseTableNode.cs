using System;
using System.Threading;

namespace Engine
{
    public abstract class BaseTableNode: Node
    {
        public string TableName;

        protected BaseTableNode(string tableName)
        {
            TableName = tableName;
        }

        public override Result Execute(Database db)
        {
            var table = GetTable(db, TableName);

            if (!Monitor.TryEnter(table.SyncObject, LockTimeout))
                throw Exceptions.FailedToLockTable(TableName);

            try
            {
                return ExecuteInternal(table);
            }
            finally
            {
                Monitor.Exit(table.SyncObject);
            }
        }

        protected abstract Result ExecuteInternal(Table table);
    }
}
