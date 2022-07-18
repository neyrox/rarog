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

        public override void Insert(long idx, string value, IStorage storage)
        {
            var val = Clamp(value);

            storage.InsertVarChars(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectVarChars(GetDataFileName(TablePath, Name), new ConditionAny<string>(), 0) 
                : storage.SelectVarChars(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            return new ResultColumnString(Name, stored.Values.ToArray());
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

        protected override void DeleteInternal(SortedSet<long> idxsToDelete, IStorage storage)
        {
            storage.DeleteVarChars(GetDataFileName(TablePath, Name), new SortedSet<long>(idxsToDelete));
        }

        protected override IReadOnlyDictionary<long, string> SelectInternal(Condition<string> cond, int limit, IStorage storage)
        {
            return storage.SelectVarChars(GetDataFileName(TablePath, Name), cond, limit);
        }

        protected override OperationGeneric<string> Transform(OperationNode opNode)
        {
            return OperationString.Transform(opNode);
        }

        protected override void UpdateInternal(long idx, OperationGeneric<string> op, IStorage storage)
        {
            storage.UpdateVarChars(GetDataFileName(TablePath, Name), idx, op);
        }
    }
}
