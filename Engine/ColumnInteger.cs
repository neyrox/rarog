using System.Collections.Generic;

namespace Engine
{
    public class ColumnInteger: Column
    {
        private List<int> values = new List<int>();

        public override void FullUpdate(string value)
        {
            int val = int.Parse(value);
            for (int row = 0; row < values.Count; ++row)
                values[row] = val;
        }

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

        public override ResultColumnBase Get(List<int> rows)
        {
            var resultValues = new int[rows.Count];
            for (int i = 0; i < rows.Count; ++i)
            {
                resultValues[i] = values[rows[i]];
            }

            return new ResultColumnInteger(resultValues);
        }
    }
}
