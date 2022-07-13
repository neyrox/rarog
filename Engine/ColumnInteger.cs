using System.Collections.Generic;
using System.Linq;
using Engine.Storage;

namespace Engine
{
    public class ColumnInteger: ColumnBase<int>
    {
        public const string TypeName = "Int";
        public override string DefaultValue => "0";
        public override string TypeNameP => TypeName;

        public ColumnInteger(string tablePath, string name, SortedDictionary<long, int> idxValues = null)
            : base(tablePath, name, idxValues)
        {
        }

        public override void Update(long idx, string value, IStorage storage)
        {
            int val = int.Parse(value);
            idxValues[idx] = val;

            storage.UpdateInts(GetDataFileName(TablePath, Name), idx, val);
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            int val = int.Parse(value);
            idxValues.Add(idx, val);
            // TODO: cleanup cache
            
            storage.InsertInts(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var indicesToLoad = GetIndicesToLoad(indices);

            var stored = storage.SelectInts(GetDataFileName(TablePath, Name), indicesToLoad);
            foreach (var iv in stored)
                idxValues[iv.Key] = iv.Value;

            if (indices == null)
                return new ResultColumnInteger(Name, idxValues.Values.ToArray());

            var resultValues = new int[indices.Count];
            for (int i = 0; i < indices.Count; ++i)
                resultValues[i] = idxValues[indices[i]];
            // TODO: cleanup cache

            return new ResultColumnInteger(Name, resultValues);
        }

        public override List<long> Filter(string op, string value, IStorage storage)
        {
            var result = new List<long>();

            var condition = ConditionInteger.Transform(op, value);
            if (condition == null)
                return result;

            var stored = storage.SelectInts(GetDataFileName(TablePath, Name), condition);
            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
        }

        public override void DeleteInternal(List<long> rowsToDelete, IStorage storage)
        {
            storage.DeleteInts(GetDataFileName(TablePath, Name), new SortedSet<long>(rowsToDelete));
        }
    }
}
