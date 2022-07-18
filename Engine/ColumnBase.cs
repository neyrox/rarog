using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public abstract class ColumnBase<T>: Column where T: IComparable<T>
    {
        protected ColumnBase(string tablePath, string name)
            : base(tablePath, name)
        {
        }

        protected SortedSet<long> GetIndicesToLoad(List<long> indices)
        {
            return new SortedSet<long>(indices);
        }

        public override List<long> AllIndices(IStorage storage, int limit)
        {
            var stored = SelectInternal(ConditionAny<T>.Instance, limit, storage);

            return stored.Keys.ToList();
        }

        public override List<long> Filter(string op, string value, IStorage storage, int limit)
        {
            var condition = Condition<T>.Transform(op, value);

            var result = new List<long>();

            var stored = SelectInternal(condition, limit, storage);
            foreach (var iv in stored)
                result.Add(iv.Key);

            return result;
        }


        public override void Update(long idx, OperationNode opNode, IStorage storage)
        {
            UpdateInternal(idx, Transform(opNode), storage);
        }

        // rowsToDelete is a sorted set of indices
        public override void Delete(SortedSet<long> rowsToDelete, IStorage storage)
        {
            if (rowsToDelete == null)
            {
                storage.DeleteFile(GetDataFileName(TablePath, Name));
                return;
            }

            DeleteInternal(rowsToDelete, storage);
        }

        protected abstract IReadOnlyDictionary<long, T> SelectInternal(Condition<T> cond, int limit, IStorage storage);

        protected abstract OperationGeneric<T> Transform(OperationNode opNode);
        protected abstract void UpdateInternal(long idx, OperationGeneric<T> op, IStorage storage);
    }
}
