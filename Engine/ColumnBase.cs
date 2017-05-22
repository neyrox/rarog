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

    }
}
