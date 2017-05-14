using System.Collections.Generic;

namespace Engine
{
    public class ResultColumnString: ResultColumnBase
    {
        private readonly string[] values;

        public override int Count { get { return values.Length; } }

        public ResultColumnString(string[] vals)
        {
            values = vals;
        }

        public override string Get(int index)
        {
            return values[index];
        }
    }
}
