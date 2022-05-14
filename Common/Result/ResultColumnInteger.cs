namespace Engine
{
    public class ResultColumnInteger: ResultColumnBase<int>
    {
        public ResultColumnInteger(string name, int[] vals)
            : base(name, vals)
        {
        }

        public override string Get(int index)
        {
            return values[index].ToString();
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
