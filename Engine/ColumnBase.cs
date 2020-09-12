using System.Collections.Generic;

namespace Engine
{
    public abstract class ColumnBase<T>: Column
    {
        protected List<T> values = new List<T>();

        protected void FullUpdateBase(T value)
        {
            for (int row = 0; row < values.Count; ++row)
                values[row] = value;
        }

        // Precondition: rowsToDelete is a sorted list of indices
        public override void Delete(List<int> rowsToDelete)
        {
            for (int i = rowsToDelete.Count - 1; i >= 0; i--)
            {
                var rowToDelete = rowsToDelete[i];
                values.RemoveAt(rowToDelete);
            }
        }
    }
}
