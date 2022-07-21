using System.Collections.Generic;

namespace Engine
{
    public class InsertNode: BaseTableNode
    {
        public List<string> ColumnNames;
        public List<string> Values;

        public InsertNode(string tableName, List<string> columnNames, List<string> values)
            : base(tableName)
        {
            ColumnNames = columnNames;
            Values = values;
        }

        protected override Result ExecuteInternal(Table table)
        {
            table.Insert(ColumnNames, Values);
            return Result.OK;
        }
    }
}
