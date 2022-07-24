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

        public abstract void Update(long idx, OperationNode op);

        public abstract void Insert(long idx, string val);

        public abstract ResultColumn Get(List<long> indices);

        public abstract List<long> AllIndices(int limit);
        public abstract List<long> Filter(string op, string value, int limit);

        public abstract void Delete(SortedSet<long> indicesToDelete, IStorage storage);

        public void Drop(IStorage storage)
        {
            DropInternal();

            storage.DeleteFile(GetMetaFileName(TablePath, Name));
            storage.DeleteFile(GetDataFileName(TablePath, Name));
        }

        protected abstract void DropInternal();

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
