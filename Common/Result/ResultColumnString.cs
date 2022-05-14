namespace Engine
{
    public class ResultColumnString: ResultColumnBase<string>
    {
        public ResultColumnString(string name, string[] vals)
            : base(name, vals)
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
