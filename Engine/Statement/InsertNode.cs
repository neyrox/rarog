using System.Collections.Generic;

namespace Engine
{
    public class InsertNode: BaseTableNode
    {
        public readonly List<string> ColumnNames;
        public readonly List<string> Values;

        public InsertNode(string tableName, List<string> columnNames, List<string> values)
            : base(tableName)
        {
            ColumnNames = columnNames;
            Values = values;
        }

        protected override Result ExecuteInternal(Table table, ref Transaction tx)
        {
            table.Insert(ColumnNames, Values);
            return Result.OK;
        }
    }
}
