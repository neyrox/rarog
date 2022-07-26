using System.Globalization;

namespace Engine
{
    public class ResultColumnDouble: ResultColumnBase<double>
    {
        public const string TypeTag = "Dbl";

        public ResultColumnDouble(string name, double[] vals)
            : base(name, vals)
        {
        }

        public override string Get(int index)
        {
            if (index >= Values.Length)
                return string.Empty;

            return Values[index].ToString(CultureInfo.InvariantCulture);
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
