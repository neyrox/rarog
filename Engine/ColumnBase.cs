using System.Collections.Generic;

namespace Engine
{
    public abstract class ColumnBase<T>: Column
    {
        protected SortedDictionary<long, T> idxValues;

        public override int Count => idxValues.Count;

        public override IReadOnlyCollection<long> Indices => idxValues.Keys;
        public IReadOnlyDictionary<long, T> IdxValues => idxValues;
        
        protected ColumnBase(string name, SortedDictionary<long, T> idxValues)
            : base(name)
        {
            this.idxValues = idxValues ?? new SortedDictionary<long, T>();
        }

        protected void FullUpdateBase(T value)
        {
            foreach (var idx in idxValues.Keys)
                idxValues[idx] = value;
        }

        // Precondition: rowsToDelete is a sorted list of indices
        public override void Delete(List<long> rowsToDelete)
        {
            if (rowsToDelete == null)
            {
                idxValues.Clear();
                return;
            }

            for (int i = rowsToDelete.Count - 1; i >= 0; i--)
            {
                var rowToDelete = rowsToDelete[i];

                idxValues.Remove(rowToDelete);
            }
        }
    }
}
