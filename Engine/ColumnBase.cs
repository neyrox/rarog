using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class ColumnBase<T>: Column where T: IComparable<T>
    {
        public override string TypeNameP => traits.TypeName;
        public override string DefaultValue => traits.DefaultValue;
        private readonly TypeTraits<T> traits;
        private readonly int maxLength;

        public ColumnBase(string tablePath, string name, TypeTraits<T> traits, int maxLength = 0)
            : base(tablePath, name)
        {
            this.traits = traits;
            this.maxLength = maxLength;
        }

        public override ResultColumn Get(List<long> indices)
        {
            var stored = indices == null
                ? traits.PageStorage.Select(GetDataFileName(TablePath, Name), new ConditionAny<T>(), 0) 
                : traits.PageStorage.Select(GetDataFileName(TablePath, Name), new SortedSet<long>(indices));

            return traits.Results.Create(Name, stored.Values.ToArray());
        }

        public override void Insert(long idx, string value)
        {
            var val = traits.Converter.FromString(value, maxLength);

            traits.PageStorage.Insert(GetDataFileName(TablePath, Name), idx, val);
        }

        public override List<long> AllIndices(int limit)
        {
            var stored = traits.PageStorage.Select(GetDataFileName(TablePath, Name), ConditionAny<T>.Instance, limit);

            return stored.Keys.ToList();
        }

        public override List<long> Filter(string op, string value, int limit)
        {
            var condition = Condition<T>.Transform(op, value);

            var result = new List<long>();

            var stored = traits.PageStorage.Select(GetDataFileName(TablePath, Name), condition, limit);
            foreach (var iv in stored)
                result.Add(iv.Key);

            return result;
        }

        public override void Update(long idx, OperationNode opNode)
        {
            traits.PageStorage.Update(GetDataFileName(TablePath, Name), idx, traits.Operations.Transform(opNode));
        }

        // rowsToDelete is a sorted set of indices
        public override void Delete(SortedSet<long> rowsToDelete, IStorage storage)
        {
            if (rowsToDelete == null)
            {
                storage.DeleteFile(GetDataFileName(TablePath, Name));
                return;
            }

            traits.PageStorage.Delete(GetDataFileName(TablePath, Name), new SortedSet<long>(rowsToDelete));
        }

        protected override void DropInternal()
        {
            traits.PageStorage.Delete(GetDataFileName(TablePath, Name));
        }
    }
}
