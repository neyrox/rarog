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

        public override ResultColumnBase Get(List<int> rows)
        {
            var resultValues = new int[rows.Count];
            for (int i = 0; i < rows.Count; ++i)
            {
                resultValues[i] = values[rows[i]];
            }

            return new ResultColumnInteger(resultValues);
        }

        public override List<int> Filter(ConditionNode conditionNode)
        {
            var result = new List<int>();

            var condition = ConditionInteger.Transform(conditionNode);
            if (condition == null)
                return result;

            for (int row = 0; row < values.Count; ++row)
            {
                var value = values[row];
                if (condition.Satisfies(value))
                    result.Add(row);
            }

            return result;
        }
    }
}
