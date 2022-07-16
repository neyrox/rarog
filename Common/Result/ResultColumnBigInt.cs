namespace Engine
{
    public class ResultColumnBigInt: ResultColumnBase<long>
    {
        public const string TypeTag = "BigInt";

        public ResultColumnBigInt(string name, long[] vals)
            : base(name, vals)
        {
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
