using System.Collections.Generic;

namespace Engine
{
    public abstract class Column
    {
        public string Name;

        public abstract int Count { get; }
        public abstract IReadOnlyCollection<int> Indices { get; }

        public abstract string DefaultValue { get; }

        public abstract void FullUpdate(string value);

        public abstract void Update(int idx, string value);

        public abstract void Insert(int idx, string value);

        public abstract ResultColumn Get(List<int> idxs);

        public abstract List<int> Filter(string op, string value);

        public abstract void Delete(List<int> idxsToDelete);
    }
}
