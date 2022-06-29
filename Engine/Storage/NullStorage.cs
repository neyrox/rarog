namespace Engine.Storage
{
    public class NullStorage : IStorage
    {
        public Column LoadColumn(string fileName)
        {
            return null;
        }

        public void Store(ColumnInteger column, string filename)
        {
        }

        public void Store(ColumnDouble column, string filename)
        {
        }

        public void Store(ColumnVarChar column, string filename)
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
    }
}
