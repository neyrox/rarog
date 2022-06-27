using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class ColumnInteger: ColumnBase<int>
    {
        public override string DefaultValue => "0";

        public ColumnInteger(string name)
        {
            Name = name;
        }

        public override void FullUpdate(string value)
        {
            int val = int.Parse(value);
            FullUpdateBase(val);
        }

        public override void Update(long idx, string value)
        {
            int val = int.Parse(value);
            idxValues[idx] = val;
        }

        public override void Insert(long idx, string value)
        {
            int val = int.Parse(value);
            idxValues.Add(idx, val);
        }

        public override ResultColumn Get(List<long> idxs)
        {
            if (idxs == null)
                return new ResultColumnInteger(Name, idxValues.Values.ToArray());

            var resultValues = new int[idxs.Count];
            for (int i = 0; i < idxs.Count; ++i)
            {
                resultValues[i] = idxValues[idxs[i]];
            }

            return new ResultColumnInteger(Name, resultValues);
        }

        public override List<long> Filter(string op, string value)
        {
            var result = new List<long>();

            var condition = ConditionInteger.Transform(op, value);
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
