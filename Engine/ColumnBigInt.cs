using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class ColumnBigInt: ColumnBase<long>
    {
        public override string DefaultValue => "0";
        public override string TypeNameP => ResultColumnBigInt.TypeTag;

        public ColumnBigInt(string tablePath, string name)
            : base(tablePath, name)
        {
        }

        public override void Update(long idx, OperationNode opNode, IStorage storage)
        {
            var op = OperationBigInt.Transform(opNode);
            if (idxValues.ContainsKey(idx))
                idxValues[idx] = op.Perform(idxValues[idx]);

            storage.UpdateBigInts(GetDataFileName(TablePath, Name), idx, op);
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            long val = long.Parse(value ?? DefaultValue);
            idxValues.Add(idx, val);
            // TODO: cleanup cache
            
            storage.InsertBigInts(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectBigInts(GetDataFileName(TablePath, Name), ConditionAny<long>.Instance,0)
                : storage.SelectBigInts(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            foreach (var iv in stored)
                idxValues[iv.Key] = iv.Value;

            if (indices == null)
                return new ResultColumnBigInt(Name, idxValues.Values.ToArray());

            var resultValues = new long[indices.Count];
            for (int i = 0; i < indices.Count; ++i)
                resultValues[i] = idxValues[indices[i]];
            // TODO: cleanup cache

            return new ResultColumnBigInt(Name, resultValues);
        }

        public override List<long> AllIndices(IStorage storage, int limit)
        {
            var result = new List<long>();

            var stored = storage.SelectBigInts(
                GetDataFileName(TablePath, Name), ConditionAny<long>.Instance, limit);

            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
        }

        public override List<long> Filter(string op, string value, IStorage storage, int limit)
        {
            var result = new List<long>();

            var condition = Condition<long>.Transform(op, value);
            if (condition == null)
                return result;

            var stored = storage.SelectBigInts(GetDataFileName(TablePath, Name), condition, limit);
            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
        }

        public override void DeleteInternal(List<long> rowsToDelete, IStorage storage)
        {
            storage.DeleteBigInts(GetDataFileName(TablePath, Name), new SortedSet<long>(rowsToDelete));
        }
    }
}
