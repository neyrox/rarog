using System.Collections.Generic;

namespace Engine
{
    public abstract class Column
    {
        public string Name;

        public abstract int Count { get; }

        public abstract string DefaultValue { get; }

        public abstract void FullUpdate(string value);

        public abstract void Update(int row, string value);

        public abstract void Insert(string value);

        public abstract ResultColumn Get(List<int> rows);

        public abstract List<int> Filter(string op, string value);

        public abstract void Delete(List<int> rowsToDelete);
    }
}
