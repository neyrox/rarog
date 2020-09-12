using System.Globalization;

namespace Engine
{
    public class ResultColumnDouble: ResultColumnBase<double>
    {
        public ResultColumnDouble(double[] vals)
            : base(vals)
        {
        }

        public override string Get(int index)
        {
            return values[index].ToString(CultureInfo.InvariantCulture);
        }
    }
}
