using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class ColumnInteger: ColumnBase<int>
    {
        public override string DefaultValue => "0";
        public override string TypeNameP => ResultColumnInteger.TypeTag;

        public ColumnInteger(string tablePath, string name)
            : base(tablePath, name)
        {
        }

        public override void Insert(long idx, string value, IStorage storage)
        {
            var val = int.Parse(value);

            storage.InsertInts(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectInts(GetDataFileName(TablePath, Name), ConditionAny<int>.Instance,0) 
                : storage.SelectInts(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            return new ResultColumnInteger(Name, stored.Values.ToArray());
        }

        protected override void DropInternal(IStorage storage)
        {
            storage.DeleteIntColumn(GetDataFileName(TablePath, Name));
        }

        protected override void DeleteInternal(SortedSet<long> rowsToDelete, IStorage storage)
        {
            storage.DeleteInts(GetDataFileName(TablePath, Name), new SortedSet<long>(rowsToDelete));
        }

        protected override IReadOnlyDictionary<long, int> SelectInternal(Condition<int> cond, int limit, IStorage storage)
        {
            return storage.SelectInts(GetDataFileName(TablePath, Name), cond, limit);
        }

        protected override OperationGeneric<int> Transform(OperationNode opNode)
        {
            return OperationInteger.Transform(opNode);
        }

        protected override void UpdateInternal(long idx, OperationGeneric<int> op, IStorage storage)
        {
            storage.UpdateInts(GetDataFileName(TablePath, Name), idx, op);
        }
    }
}
