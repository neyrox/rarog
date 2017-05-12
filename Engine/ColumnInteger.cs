using System.Collections.Generic;

namespace Engine
{
    public class ColumnInteger: Column
    {
        private List<int> values = new List<int>();

        public override void Update(int row, string value)
        {
            int val = int.Parse(value);
            values[row] = val;
        }

        public override void Insert(string value)
        {
            int val = int.Parse(value);
            values.Add(val);
        }

        public override List<int> GetInclusions(string value)
        {
            var result = new List<int>();
            int val = int.Parse(value);
            for (int row = 0; row < values.Count; ++row)
            {
                if (values[row] == val)
                    result.Add(row);
            }

            return result;
        }

        public override string Get(int index)
        {
            return values[index].ToString();
        }
    }
}
