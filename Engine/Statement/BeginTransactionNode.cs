using System;

namespace Engine
{
    public class BeginTransactionNode: Node
    {
        public override Result Execute(Database db, ref Transaction tx)
        {
            if (!tx.IsSingleQuery)
                throw new Exception("Another transaction is in process");

            tx = new ComplexTransaction();

            return Result.OK;
        }
    }
}
