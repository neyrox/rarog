using System.Globalization;

namespace Engine
{
    public class ResultColumnDouble: ResultColumnBase<double>
    {
        public ResultColumnDouble(string name, double[] vals)
            : base(name, vals)
        {
        }

        public override string Get(int index)
        {
            return values[index].ToString(CultureInfo.InvariantCulture);
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
