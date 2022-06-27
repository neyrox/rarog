using System.Collections.Generic;

namespace Engine
{
    public abstract class Column
    {
        public string Name;

        public abstract int Count { get; }
        public abstract IReadOnlyCollection<long> Indices { get; }

        public abstract string DefaultValue { get; }

        public abstract void FullUpdate(string value);

        public abstract void Update(long idx, string value);

        public abstract void Insert(long idx, string value);

        public abstract ResultColumn Get(List<long> idxs);

        public abstract List<long> Filter(string op, string value);

        public abstract void Delete(List<long> idxsToDelete);
    }
}
