using System.Collections.Generic;
using System.IO;
using Engine.Storage;

namespace Engine
{
    public abstract class Column
    {
        public const string MetaFileExtension = ".meta";
        public const string DataFileExtension = ".data";

        public string TablePath;
        public string Name;

        public abstract int Count { get; }
        public abstract IReadOnlyCollection<long> Indices { get; }

        public abstract string DefaultValue { get; }
        public abstract string TypeNameP { get; }

        protected Column(string tablePath, string name)
        {
            TablePath = tablePath;
            Name = name;
        }

        public abstract void Update(long idx, string value, IStorage storage);

        public abstract void Insert(long idx, string value, IStorage storage);

        public abstract ResultColumn Get(List<long> indices, IStorage storage);

        public abstract List<long> Filter(string op, string value, IStorage storage);

        public abstract void Delete(List<long> indicesToDelete, IStorage storage);
        public abstract void DeleteInternal(List<long> indicesToDelete, IStorage storage);
        

        public static string GetMetaFileName(string path, string name)
        {
            return Path.Combine(path, name + MetaFileExtension);
        }
        public static string GetDataFileName(string path, string name)
        {
            return Path.Combine(path, name + DataFileExtension);
        }
    }
}
