using System.Collections.Generic;
using System.Linq;
using Engine.Storage;

namespace Engine
{
    public class ColumnVarChar: ColumnBase<string>
    {
        public int MaxLength = 1000000;

        public override string DefaultValue => "";

        public ColumnVarChar(string name, SortedDictionary<long, string> idxValues = null)
            : base(name, idxValues)
        {
        }

        public override void FullUpdate(string value)
        {
            var val = Clamp(value);
            FullUpdateBase(val);
        }

        public override void Update(long idx, string value)
        {
            idxValues[idx] = Clamp(value);
        }

        public override void Insert(long idx, string value)
        {
            idxValues.Add(idx, Clamp(value));
        }

        public override ResultColumn Get(List<long> idxs)
        {
            if (idxs == null)
                return new ResultColumnString(Name, idxValues.Values.ToArray());

            var resultValues = new string[idxs.Count];
            for (int i = 0; i < idxs.Count; ++i)
            {
                resultValues[i] = idxValues[idxs[i]];
            }

            return new ResultColumnString(Name, resultValues);
        }

        private string Clamp(string value)
        {
            if (value.Length < MaxLength)
            {
                return value;
            }

            // TODO: produce warning if string is too long
            return value.Substring(0, MaxLength);
        }

        public override List<long> Filter(string operation, string value)
        {
            var result = new List<long>();

            var condition = ConditionString.Transform(operation, value);
            if (condition == null)
                return result;

            foreach (var iv in idxValues)
            {
                if (condition.Satisfies(iv.Value))
                    result.Add(iv.Key);
            }

            return result;
        }

        public override void Store(IStorage storage, string path)
        {
            storage.Store(this, path);
        }
    }
}
