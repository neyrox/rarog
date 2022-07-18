using System.Collections.Generic;
using Engine.Statement;

namespace Engine.Storage
{
    public interface IStorage
    {
        void StoreTableMeta(string fileName, long nextIdx);
        void LoadTableMeta(string fileName, out long nextIdx);

        void StoreColumnMeta(Column column, string tableName, string fileName);
        Column LoadColumnMeta(string tablePath, string fileName);

        IReadOnlyDictionary<long, int> SelectInts(string fileName, Condition<int> cond, int limit);
        IReadOnlyDictionary<long, int> SelectInts(string fileName, SortedSet<long> indices);
        IReadOnlyDictionary<long, long> SelectBigInts(string fileName, Condition<long> cond, int limit);
        IReadOnlyDictionary<long, long> SelectBigInts(string fileName, SortedSet<long> indices);
        IReadOnlyDictionary<long, double> SelectDoubles(string fileName, Condition<double> cond, int limit);
        IReadOnlyDictionary<long, double> SelectDoubles(string fileName, SortedSet<long> indices);
        IReadOnlyDictionary<long, string> SelectVarChars(string fileName, Condition<string> cond, int limit);
        IReadOnlyDictionary<long, string> SelectVarChars(string fileName, SortedSet<long> indices);

        void UpdateInts(string fileName, long idx, OperationGeneric<int> op);
        void UpdateBigInts(string fileName, long idx, OperationGeneric<long> op);
        void UpdateDoubles(string fileName, long idx, OperationGeneric<double> op);
        void UpdateVarChars(string fileName, long idx, OperationGeneric<string> op);

        void InsertInts(string fileName, long idx, int val);
        void InsertBigInts(string fileName, long idx, long val);
        void InsertDoubles(string fileName, long idx, double val);
        void InsertVarChars(string fileName, long idx, string val);

        void DeleteInts(string fileName, SortedSet<long> indices);
        void DeleteBigInts(string fileName, SortedSet<long> indices);
        void DeleteDoubles(string fileName, SortedSet<long> indices);
        void DeleteVarChars(string fileName, SortedSet<long> indices);

        string[] GetTableNames();

        string[] GetColumnFiles(string tableDir);

        void CreateDirectoryIfNotExist(string tableDir);

        void DeleteFile(string fileName);

        void DeleteDirectory(string tableDir);

        void Flush();
    }
}
