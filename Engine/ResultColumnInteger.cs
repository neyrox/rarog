using System.Collections.Generic;

namespace Engine
{
    public class ResultColumnInteger: ResultColumnBase<int>
    {
        public ResultColumnInteger(int[] vals)
            : base(vals)
        {
        }

        public override string Get(int index)
        {
            return values[index].ToString();
        }
    }
}
