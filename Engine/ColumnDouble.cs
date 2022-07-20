using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class ColumnDouble: ColumnBase<double>
    {
        public override string DefaultValue => "0";
        public override string TypeNameP => ResultColumnDouble.TypeTag;

        public ColumnDouble(string tablePath, string name)
            : base(tablePath, name)
        {
        }
        
        public override void Insert(long idx, string value, IStorage storage)
        {
            var val = double.Parse(value);

            storage.InsertDoubles(GetDataFileName(TablePath, Name), idx, val);
        }

        public override ResultColumn Get(List<long> indices, IStorage storage)
        {
            var stored = indices == null
                ? storage.SelectDoubles(GetDataFileName(TablePath, Name), ConditionAny<double>.Instance,0) 
                : storage.SelectDoubles(GetDataFileName(TablePath, Name), GetIndicesToLoad(indices));

            return new ResultColumnDouble(Name, stored.Values.ToArray());
        }

        protected override void DeleteInternal(SortedSet<long> indicesToDelete, IStorage storage)
        {
            storage.DeleteDoubles(GetDataFileName(TablePath, Name), new SortedSet<long>(indicesToDelete));
        }

        protected override void DeleteSelfInternal(IStorage storage)
        {
            storage.DeleteDoubleColumn(GetDataFileName(TablePath, Name));
        }

        protected override IReadOnlyDictionary<long, double> SelectInternal(Condition<double> cond, int limit, IStorage storage)
        {
            return storage.SelectDoubles(GetDataFileName(TablePath, Name), cond, limit);
        }

        protected override OperationGeneric<double> Transform(OperationNode opNode)
        {
            return OperationDouble.Transform(opNode);
        }

        protected override void UpdateInternal(long idx, OperationGeneric<double> op, IStorage storage)
        {
            storage.UpdateDoubles(GetDataFileName(TablePath, Name), idx, op);
        }
    }
}
