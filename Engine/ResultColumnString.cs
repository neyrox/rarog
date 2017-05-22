using System.Collections.Generic;

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
    }
}
