using System.Collections.Generic;
using System.Linq;
using Engine.Storage;

namespace Engine
{
    public class ColumnDouble: ColumnBase<double>
    {
        public const string TypeName = "Dbl";
        public override string DefaultValue => "0";
        public override string TypeNameP => TypeName;

        public ColumnDouble(string tablePath, string name, SortedDictionary<long, double> idxValues = null)
            : base(tablePath, name, idxValues)
        {
        }

        public override void Update(long idx, string value, IStorage storage)
        {
            var val = double.Parse(value);
            idxValues[idx] = val;

            storage.UpdateDoubles(GetDataFileName(TablePath, Name), idx, val);
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            var val = double.Parse(value);
            idxValues.Add(idx, val);
            // TODO: cleanup cache
            
            storage.InsertDoubles(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var indicesToLoad = GetIndicesToLoad(indices);

            var stored = storage.SelectDoubles(GetDataFileName(TablePath, Name), indicesToLoad);
            foreach (var iv in stored)
                idxValues[iv.Key] = iv.Value;

            if (indices == null)
                return new ResultColumnDouble(Name, idxValues.Values.ToArray());


            var resultValues = new double[indices.Count];
            for (int i = 0; i < indices.Count; ++i)
                resultValues[i] = idxValues[indices[i]];
            // TODO: cleanup cache

            return new ResultColumnDouble(Name, resultValues);
        }

        public override List<long> Filter(string op, string value, IStorage storage)
        {
            var result = new List<long>();

            var condition = ConditionDouble.Transform(op, value);
            if (condition == null)
                return result;

            var stored = storage.SelectDoubles(GetDataFileName(TablePath, Name), condition);
            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
        }

        public override void DeleteInternal(List<long> idxsToDelete, IStorage storage)
        {
            storage.DeleteDoubles(GetDataFileName(TablePath, Name), new SortedSet<long>(idxsToDelete));
        }
    }
}
