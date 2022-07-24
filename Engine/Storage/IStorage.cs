namespace Engine.Storage
{
    public interface IStorage: IStreamProvider
    {
        void StoreTableMeta(string fileName, long nextIdx);
        void LoadTableMeta(string fileName, out long nextIdx);

        void StoreColumnMeta(Column column, string tableName, string fileName);
        Column LoadColumnMeta(string tablePath, string fileName, Registry registry);

        string[] GetTableNames();

        string[] GetColumnFiles(string tableDir);

        void CreateDirectoryIfNotExist(string tableDir);

        void DeleteFile(string fileName);

        void DeleteDirectory(string tableDir);
    }
}
