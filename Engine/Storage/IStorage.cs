namespace Engine.Storage
{
    public interface IStorage
    {
        Column LoadColumn(string fileName);

        void Store(ColumnInteger column, string tableDir);

        void Store(ColumnDouble column, string tableDir);

        void Store(ColumnVarChar column, string tableDir);

        string[] GetTableNames();

        string[] GetColumnFiles(string tableDir);

        void CreateDirectoryIfNotExist(string tableDir);

        void DeleteFile(string fileName);
    }
}
