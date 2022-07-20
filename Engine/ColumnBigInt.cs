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

        public override void Insert(long idx, string value, IStorage storage)
        {
            var val = long.Parse(value);

            storage.InsertBigInts(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectBigInts(GetDataFileName(TablePath, Name), ConditionAny<long>.Instance,0) 
                : storage.SelectBigInts(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            return new ResultColumnBigInt(Name, stored.Values.ToArray());
        }

        protected override void DeleteInternal(SortedSet<long> rowsToDelete, IStorage storage)
        {
            storage.DeleteBigInts(GetDataFileName(TablePath, Name), new SortedSet<long>(rowsToDelete));
        }

        protected override void DeleteSelfInternal(IStorage storage)
        {
            storage.DeleteBigIntColumn(GetDataFileName(TablePath, Name));
        }

        protected override IReadOnlyDictionary<long, long> SelectInternal(Condition<long> cond, int limit, IStorage storage)
        {
            return storage.SelectBigInts(GetDataFileName(TablePath, Name), cond, limit);
        }

        protected override OperationGeneric<long> Transform(OperationNode opNode)
        {
            return OperationBigInt.Transform(opNode);
        }

        protected override void UpdateInternal(long idx, OperationGeneric<long> op, IStorage storage)
        {
            storage.UpdateBigInts(GetDataFileName(TablePath, Name), idx, op);
        }
    }
}
