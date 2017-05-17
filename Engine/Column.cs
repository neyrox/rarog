using System.Collections.Generic;

namespace Engine
{
    public abstract class Column
    {
        public abstract void FullUpdate(string value);

        public abstract void Update(int row, string value);

        public abstract void Insert(string value);

        public abstract ResultColumnBase Get(List<int> rows);

        public abstract List<int> Filter(string op, string value);

        public abstract void Delete(List<int> rowsToDelete);
    }
}
