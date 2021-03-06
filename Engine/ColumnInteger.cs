﻿using System.Collections.Generic;

namespace Engine
{
    public class ColumnInteger: ColumnBase<int>
    {
        public override string DefaultValue => "0";

        public override void FullUpdate(string value)
        {
            int val = int.Parse(value);
            FullUpdateBase(val);
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

        public override ResultColumn Get(List<int> rows)
        {
            var resultValues = new int[rows.Count];
            for (int i = 0; i < rows.Count; ++i)
            {
                resultValues[i] = values[rows[i]];
            }

            return new ResultColumnInteger(resultValues);
        }

        public override List<int> Filter(string op, string value)
        {
            var result = new List<int>();

            var condition = ConditionInteger.Transform(op, value);
            if (condition == null)
                return result;

            for (int row = 0; row < values.Count; ++row)
            {
                var val = values[row];
                if (condition.Satisfies(val))
                    result.Add(row);
            }

            return result;
        }
    }
}
