using System;
using System.Threading;

namespace Engine
{
    public abstract class BaseTableNode: Node
    {
        public readonly string TableName;

        protected BaseTableNode(string tableName)
        {
            TableName = tableName;
        }

        public override Result Execute(Database db, ref Transaction tx)
        {
            var table = GetTable(db, TableName);

            if (!tx.TryLock(table.Sem, LockTimeout))
                throw Exceptions.FailedToLockTable(TableName);

            return ExecuteInternal(table, ref tx);
        }

        protected abstract Result ExecuteInternal(Table table, ref Transaction tx);
    }
}
