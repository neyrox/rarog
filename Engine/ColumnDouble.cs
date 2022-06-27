using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class ColumnDouble: ColumnBase<double>
    {
        public override string DefaultValue => "0";

        public ColumnDouble(string name)
        {
            Name = name;
        }

        public override void FullUpdate(string value)
        {
            var val = double.Parse(value);
            FullUpdateBase(val);
        }

        public override void Update(long idx, string value)
        {
            var val = double.Parse(value);
            idxValues[idx] = val;
        }

        public override void Insert(long idx, string value)
        {
            var val = double.Parse(value);
            idxValues.Add(idx, val);
        }

        public override ResultColumn Get(List<long> idxs)
        {
            if (idxs == null)
                return new ResultColumnDouble(Name, idxValues.Values.ToArray());

            var resultValues = new double[idxs.Count];
            for (int i = 0; i < idxs.Count; ++i)
            {
                resultValues[i] = idxValues[idxs[i]];
            }

            return new ResultColumnDouble(Name, resultValues);
        }

        public override List<long> Filter(string op, string value)
        {
            var result = new List<long>();

            var condition = ConditionDouble.Transform(op, value);
            if (condition == null)
                return result;

            foreach (var iv in idxValues)
            {
                if (condition.Satisfies(iv.Value))
                    result.Add(iv.Key);
            }

            return result;
        }
    }
}
