using System.Collections.Generic;
using System.IO;
using Engine.Storage;

namespace Engine
{
    public abstract class Column
    {
        public const string ColumnFileExtension = ".clmn";

        public string Name;

        public abstract int Count { get; }
        public abstract IReadOnlyCollection<long> Indices { get; }

        public abstract string DefaultValue { get; }

        protected Column(string name)
        {
            Name = name;
        }

        public abstract void FullUpdate(string value);

        public abstract void Update(long idx, string value);

        public abstract void Insert(long idx, string value);

        public abstract ResultColumn Get(List<long> idxs);

        public abstract List<long> Filter(string op, string value);

        public abstract void Delete(List<long> idxsToDelete);

        public static string GetFileName(string path, string name)
        {
            return Path.Combine(path, name + ColumnFileExtension);
        }

        public abstract void Store(IStorage storage, string path);
    }
}
