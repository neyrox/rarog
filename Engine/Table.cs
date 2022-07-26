using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class Table
    {
        public readonly object SyncObject = new object();

        private const int MaxColumnNameLength = 64;
        public const string MetaFileExtension = ".meta";
        public const string TableDirExtension = ".data";
        private readonly Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private readonly IStorage storage;
        private readonly Registry registry;
        private long nextIdx = 0;

        public string Name { get; }

        public Table(string name, IStorage storage, Registry registry)
        {
            Name = name;
            this.storage = storage;
            this.registry = registry;
        }

        public Column FirstColumn => columns.First().Value;

        public IEnumerable<string> ColumnNames => columns.Keys;

        public Column GetColumn(string name)
        {
            return columns[name];
        }

        public bool TryGetColumn(string name, out Column column)
        {
            return columns.TryGetValue(name, out column);
        }

        public void AddColumn(string name, string type, int length)
        {
            if (name.Length > MaxColumnNameLength)
                throw new Exception($"Column name {name} is too long. Maximum column name length is {MaxColumnNameLength}");

            var tablePath = GetTableDir();
            switch (type.ToLowerInvariant())
            {
                case "int":
                    AddColumn(new ColumnBase<int>(tablePath, name, registry.IntTraits));
                    break;
                case "bigint":
                    AddColumn(new ColumnBase<long>(tablePath, name, registry.BigIntTraits));
                    break;
                case "float":
                case "double":
                    AddColumn(new ColumnBase<double>(tablePath, name, registry.DoubleTraits));
                    break;
                case "varchar":
                    AddColumn(new ColumnBase<string>(tablePath, name, registry.StrTraits, length));
                    break;
                default:
                    throw new Exception($"Unknown column type {type}");
            }
        }

        public void DropColumn(string name)
        {
            if (columns.ContainsKey(name))
            {
                columns.Remove(name);
                storage.DeleteFile(Column.GetMetaFileName(GetTableDir(), name));
                storage.DeleteFile(Column.GetDataFileName(GetTableDir(), name));
            }
            else
            {
                throw new Exception($"Column '{name}' not found in table '{Name}'");
            }
        }

        public void Update(List<string> columnNames, List<OperationNode> ops, ConditionNode condition)
        {
            var rowsToUpdate = condition.GetRowsThatSatisfy(this, 0);

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var op = ops[i];

                // TODO: batch it
                for (int j = 0; j < rowsToUpdate.Count; ++j)
                {
                    long row = rowsToUpdate[j];
                    var column = columns[columnName];
                    column.Update(row, op);
                }
            }
        }

        // TODO: lock all columns
        public void Insert(List<string> columnNames, List<string> values)
        {
            var allColumnNames = new HashSet<string>(columns.Keys);
            var insertingColumnNames = new HashSet<string>(columnNames);

            foreach (var columnName in allColumnNames)
            {
                if (insertingColumnNames.Contains(columnName))
                    continue;

                columns[columnName].Insert(nextIdx, null);
            }

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                columns[columnName].Insert(nextIdx, value);
            }

            AddRow();
        }

        // TODO: lock all columns
        public void Delete(ConditionNode condition)
        {
            List<long> rowsToDelete;
            if (condition != null)
                rowsToDelete = condition.GetRowsThatSatisfy(this, 0);
            else
                // TODO: implement truncate
                rowsToDelete = FirstColumn.AllIndices(0);

            foreach (var column in columns.Values)
                column.Delete(new SortedSet<long>(rowsToDelete), storage);
        }

        public void Load()
        {
            storage.LoadTableMeta(GetTableMetaFile(), out nextIdx);
            var tableDir = GetTableDir();
            foreach (var columnFile in storage.GetColumnFiles(tableDir))
            {
                if (!columnFile.EndsWith(Column.MetaFileExtension))
                    continue;

                var column = storage.LoadColumnMeta(tableDir, columnFile, registry);
                if (column != null)
                    columns.Add(column.Name, column);
            }
            // TODO: check column indices
        }

        public void Store()
        {
            var tableDir = GetTableDir();

            storage.CreateDirectoryIfNotExist(tableDir);
            storage.StoreTableMeta(GetTableMetaFile(), nextIdx);
        }

        public string GetTableDir()
        {
            return Name + TableDirExtension;
        }

        public string GetTableMetaFile()
        {
            return Name + MetaFileExtension;
        }

        public void Drop()
        {
            storage.DeleteFile(GetTableMetaFile());

            foreach (var column in columns)
                column.Value.Drop(storage);

            storage.DeleteDirectory(GetTableDir());
        }
        
        private void AddColumn(Column column)
        {
            if (columns.Count > 0)
            {
                // TODO: optimize
                foreach (var idx in FirstColumn.AllIndices(0))
                    column.Insert(idx, column.DefaultValue);
            }

            columns.Add(column.Name, column);
            storage.StoreColumnMeta(column, Name, Column.GetMetaFileName(GetTableDir(), column.Name));
        }

        private void AddRow()
        {
            ++nextIdx;

            storage.StoreTableMeta(GetTableMetaFile(), nextIdx);
        }
    }
}
