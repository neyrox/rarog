using System.Collections.Generic;
using System.Linq;
using Engine.Storage;

namespace Engine
{
    public class ColumnVarChar: ColumnBase<string>
    {
        public const string TypeName = "Str";

        public int MaxLength = 65535;
        public override string TypeNameP => TypeName;

        public override string DefaultValue => "";

        public ColumnVarChar(string tablePath, string name, SortedDictionary<long, string> idxValues = null)
            : base(tablePath, name, idxValues)
        {
        }

        public override void Update(long idx, string value, IStorage storage)
        {
            idxValues[idx] = Clamp(value);

            storage.UpdateVarChars(GetDataFileName(TablePath, Name), idx, value);
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            idxValues.Add(idx, value == null ? DefaultValue : Clamp(value));
            // TODO: cleanup cache

            storage.InsertVarChars(GetDataFileName(TablePath, Name), idx, value);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var indicesToLoad = GetIndicesToLoad(indices);

            var stored = storage.SelectVarChars(GetDataFileName(TablePath, Name), indicesToLoad);
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

        private string Clamp(string value)
        {
            if (value.Length < MaxLength)
            {
                return value;
            }

            // TODO: produce warning if string is too long
            return value.Substring(0, MaxLength);
        }

        public override List<long> Filter(string operation, string value, IStorage storage)
        {
            var result = new List<long>();

            var condition = ConditionString.Transform(operation, value);
            if (condition == null)
                return result;

            var stored = storage.SelectVarChars(GetDataFileName(TablePath, Name), condition);
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
