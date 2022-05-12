namespace Engine
{
    public class ResultColumnString: ResultColumnBase<string>
    {
        public ResultColumnString(string[] vals)
            : base(vals)
        {
        }

        public override string Get(int index)
        {
            return values[index];
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
