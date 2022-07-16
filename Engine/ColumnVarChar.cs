using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class ColumnVarChar: ColumnBase<string>
    {
        public int MaxLength = 65535;
        public override string TypeNameP => ResultColumnString.TypeTag;

        public override string DefaultValue => "";

        public ColumnVarChar(string tablePath, string name)
            : base(tablePath, name)
        {
        }

        public override void Update(long idx, OperationNode opNode, IStorage storage)
        {
            var op = OperationString.Transform(opNode);
            if (idxValues.ContainsKey(idx))
                idxValues[idx] = op.Perform(idxValues[idx]);

            // TODO: cache new values via data callback
            storage.UpdateVarChars(GetDataFileName(TablePath, Name), idx, op);
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            idxValues.Add(idx, value == null ? DefaultValue : Clamp(value));
            // TODO: cleanup cache

            storage.InsertVarChars(GetDataFileName(TablePath, Name), idx, value);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectVarChars(GetDataFileName(TablePath, Name), new ConditionAny<string>(), 0) 
                : storage.SelectVarChars(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            foreach (var iv in stored)
                idxValues[iv.Key] = iv.Value;

            if (indices == null)
                return new ResultColumnString(Name, idxValues.Values.ToArray());

            var resultValues = new string[indices.Count];
            for (int i = 0; i < indices.Count; ++i)
                resultValues[i] = idxValues[indices[i]];
            // TODO: cleanup cache

            return new ResultColumnString(Name, resultValues);
        }

        public override List<long> AllIndices(IStorage storage, int limit)
        {
            var result = new List<long>();

            var stored = storage.SelectVarChars(
                GetDataFileName(TablePath, Name), new ConditionAny<string>(), limit);

            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
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

        public override List<long> Filter(string operation, string value, IStorage storage, int limit)
        {
            var condition = Condition<string>.Transform(operation, value);

            var result = new List<long>();

            var stored = storage.SelectVarChars(
                GetDataFileName(TablePath, Name), condition, limit);

            foreach (var iv in stored)
            {
                idxValues[iv.Key] = iv.Value;
                result.Add(iv.Key);
            }

            return result;
        }

        public override void DeleteInternal(List<long> idxsToDelete, IStorage storage)
        {
            storage.DeleteVarChars(GetDataFileName(TablePath, Name), new SortedSet<long>(idxsToDelete));
        }
    }
}
