using System.Collections.Generic;

namespace Engine.Storage
{
    public class NullStorage : IStorage
    {
        public void StoreTableMeta(string fileName, long nextIdx)
        {
        }

        public void LoadTableMeta(string fileName, out long nextIdx)
        {
            nextIdx = 0;
        }

        public void StoreColumnMeta(Column column, string tableName, string fileName)
        {
        }

        public Column LoadColumnMeta(string tablePath, string fileName)
        {
            return null;
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, ConditionInteger cond)
        {
            return new Dictionary<long, int>();
        }

        public IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices)
        {
            return new Dictionary<long, int>();
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, ConditionDouble cond)
        {
            return new Dictionary<long, double>();
        }

        public IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices)
        {
            return new Dictionary<long, double>();
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, ConditionString cond)
        {
            return new Dictionary<long, string>();
        }

        public IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices)
        {
            return new Dictionary<long, string>();
        }

        public void UpdateInts(string fileName, long idx, int val)
        {
        }

        public void UpdateDoubles(string fileName, long idx, double val)
        {
        }

        public void UpdateVarChars(string fileName, long idx, string val)
        {
        }

        public void InsertInts(string fileName, long idx, int val)
        {
        }

        public void InsertDoubles(string fileName, long idx, double val)
        {
        }

        public void InsertVarChars(string fileName, long idx, string val)
        {
        }

        public void DeleteInts(string fileName, SortedSet<long> indices)
        {
        }

        public void DeleteDoubles(string fileName, SortedSet<long> indices)
        {
        }

        public void DeleteVarChars(string fileName, SortedSet<long> indices)
        {
        }


        public string[] GetTableNames()
        {
            return new string[0];
        }

        public string[] GetColumnFiles(string path)
        {
            return new string[0];
        }

        public void CreateDirectoryIfNotExist(string tableDir)
        {
        }

        public void DeleteFile(string fileName)
        {
        }

        public void DeleteDirectory(string tableDir)
        {
        }
    }
}
