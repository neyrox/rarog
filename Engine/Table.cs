using System;
using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public class Table
    {
        public const string MetaFileExtension = ".meta";
        public const string TableDirExtension = ".data";
        private readonly Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private readonly IStorage storage;
        private long nextIdx = 0;

        public string Name { get; }

        public Table(string name, IStorage storage)
        {
            Name = name;
            this.storage = storage;
        }

        public Column GetColumn(string name)
        {
            return columns[name];
        }

        public void AddColumn(string name, string type, int length)
        {
            var tablePath = GetTableDir();
            switch (type.ToLowerInvariant())
            {
                case "int":
                    AddColumn(new ColumnInteger(tablePath, name));
                    break;
                case "float":
                case "double":
                    AddColumn(new ColumnDouble(tablePath, name));
                    break;
                case "varchar":
                    var column = new ColumnVarChar(tablePath, name);
                    column.MaxLength = length;
                    AddColumn(column);
                    break;
                default:
                    throw new Exception($"Unknown type {type}");
            }
        }

        public void DropColumn(string name)
        {
            if (columns.ContainsKey(name))
            {
                columns.Remove(name);
                storage.DeleteFile(Column.GetDataFileName(GetTableDir(), name));
            }
            else
            {
                throw new Exception($"Column '{name}' not found in table '{Name}'");
            }
        }

        public void Update(List<string> columnNames, List<string> values, ConditionNode condition)
        {
            var rowsToUpdate = condition.GetRowsThatSatisfy(this, storage);

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                for (int j = 0; j < rowsToUpdate.Count; ++j)
                {
                    long row = rowsToUpdate[j];
                    var column = columns[columnName];
                    column.Update(row, value, storage);
                }
            }
        }

        public void Insert(List<string> columnNames, List<string> values)
        {
            var allColumnNames = new HashSet<string>(columns.Keys);
            var insertingColumnNames = new HashSet<string>(columnNames);

            foreach (var columnName in allColumnNames)
            {
                if (insertingColumnNames.Contains(columnName))
                    continue;

                columns[columnName].Insert(nextIdx, null, storage);
            }

            for (int i = 0; i < columnNames.Count; ++i)
            {
                var columnName = columnNames[i];
                var value = values[i];

                columns[columnName].Insert(nextIdx, value, storage);
            }

            AddRow();
        }

        public List<ResultColumn> Select(List<string> columnNames, ConditionNode condition)
        {
            List<long> rowsToSelect = null;
            // TODO: replace with empty condition?
            if (condition != null)
            {
                rowsToSelect = condition.GetRowsThatSatisfy(this, storage);
            }

            return Select(columnNames, rowsToSelect);
        }

        public void Delete(ConditionNode condition)
        {
            List<long> rowsToDelete = null;
            // TODO: replace with empty condition?
            if (condition != null)
            {
                rowsToDelete = condition.GetRowsThatSatisfy(this, storage);
            }

            foreach (var column in columns)
            {
                column.Value.Delete(rowsToDelete, storage);
            }
        }

        public void Load()
        {
            storage.LoadTableMeta(GetTableMetaFile(), out nextIdx);
            var tableDir = GetTableDir();
            foreach (var columnFile in storage.GetColumnFiles(tableDir))
            {
                var column = storage.LoadColumnMeta(tableDir, columnFile);
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

        private void AddColumn(Column column)
        {
            if (columns.Count > 0)
            {
                using (var enumerator = columns.GetEnumerator())
                {
                    enumerator.MoveNext();
                    var firstColumn = enumerator.Current.Value;
                    var indices = firstColumn.Indices;
                    foreach (var idx in indices)
                        column.Insert(idx, column.DefaultValue, storage);
                }
            }

            columns.Add(column.Name, column);
            storage.StoreColumnMeta(column, Name, Column.GetMetaFileName(GetTableDir(), column.Name));
        }

        private List<ResultColumn> Select(List<string> columnNames, List<long> rows)
        {
            var result = new List<ResultColumn>();

            List<Column> columnsToQuery;
            if (columnNames[0] == "*")
            {
                columnsToQuery = new List<Column>(columns.Values);
            }
            else
            {
                columnsToQuery = new List<Column>();
                for (int i = 0; i < columnNames.Count; ++i)
                {
                    var columnName = columnNames[i];
                    columnsToQuery.Add(columns[columnName]);
                }
            }

            for (int j = 0; j < columnsToQuery.Count; ++j)
            {
                result.Add(columnsToQuery[j].Get(rows, storage));
            }

            return result;
        }

        private void AddRow()
        {
            ++nextIdx;

            storage.StoreTableMeta(GetTableMetaFile(), nextIdx);
        }
    }
}
