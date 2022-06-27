using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public abstract class ColumnBase<T>: Column
    {
        protected SortedDictionary<int, T> idxValues = new SortedDictionary<int, T>();

        public override int Count => idxValues.Count;

        public override IReadOnlyCollection<int> Indices => idxValues.Keys;

        protected void FullUpdateBase(T value)
        {
            foreach (var idx in idxValues.Keys)
                idxValues[idx] = value;
        }

        // Precondition: rowsToDelete is a sorted list of indices
        public override void Delete(List<int> rowsToDelete)
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
