using System.Collections.Generic;
using System.IO;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public abstract class Column
    {
        public const string MetaFileExtension = ".meta";
        public const string DataFileExtension = ".data";

        public string TablePath;
        public string Name;

        public abstract string DefaultValue { get; }
        public abstract string TypeNameP { get; }

        protected Column(string tablePath, string name)
        {
            TablePath = tablePath;
            Name = name;
        }

        public abstract void Update(long idx, OperationNode op, IStorage storage);

        public abstract void Insert(long idx, string val, IStorage storage);

        public abstract ResultColumn Get(List<long> indices, IStorage storage);

        public abstract List<long> AllIndices(IStorage storage, int limit);
        public abstract List<long> Filter(string op, string value, IStorage storage, int limit);

        public abstract void Delete(SortedSet<long> indicesToDelete, IStorage storage);
        protected abstract void DeleteInternal(SortedSet<long> indicesToDelete, IStorage storage);
        

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
