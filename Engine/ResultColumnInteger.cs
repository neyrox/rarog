using System.Collections.Generic;

namespace Engine
{
    public class ResultColumnInteger: ResultColumnBase
    {
        private readonly int[] values;

        public override int Count { get { return values.Length; } }

        public ResultColumnInteger(int[] vals)
        {
            values = vals;
        }

        public override string Get(int index)
        {
            return values[index].ToString();
        }
    }
}
