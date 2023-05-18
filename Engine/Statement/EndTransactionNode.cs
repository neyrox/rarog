namespace Engine
{
    public class EndTransactionNode: Node
    {
        public override Result Execute(Database db, ref Transaction tx)
        {
            tx.Unlock();

            tx = new SingleQueryTransaction();

            return Result.OK;
        }
    }
}
