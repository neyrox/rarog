using System.Collections.Generic;

namespace Engine
{
    public class ColumnVarChar: Column
    {
        private List<string> values = new List<string>();
        private readonly int maxLength;

        public ColumnVarChar(int maxLen)
        {
            maxLength = maxLen;
        }

        public override void FullUpdate(string value)
        {
            var val = Clamp(value);
            for (int row = 0; row < values.Count; ++row)
                values[row] = val;
        }

        public override void Update(int row, string value)
        {
            values[row] = Clamp(value);
        }

        public override void Insert(string value)
        {
            values.Add(Clamp(value));
        }

        public override List<int> GetInclusions(string value)
        {
            var result = new List<int>();
            for (int row = 0; row < values.Count; ++row)
            {
                if (values[row] == value)
                    result.Add(row);
            }

            return result;
        }

        public override ResultColumnBase Get(List<int> rows)
        {
            var resultValues = new string[rows.Count];
            for (int i = 0; i < rows.Count; ++i)
            {
                resultValues[i] = values[rows[i]];
            }

            return new ResultColumnString(resultValues);
        }

        private string Clamp(string value)
        {
            if (value.Length < maxLength)
            {
                return value;
            }
            else
            {
                // TODO: produce warning if string is too long
                return value.Substring(0, maxLength);
            }
        }
    }
}
