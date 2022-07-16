namespace Engine
{
    public class ResultColumnString: ResultColumnBase<string>
    {
        public const string TypeTag = "Str";

        public ResultColumnString(string name, string[] vals)
            : base(name, vals)
        {
        }

        public override string Get(int index)
        {
            if (index >= values.Length)
                return string.Empty;

            return values[index];
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
