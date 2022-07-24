using System.Collections.Generic;

namespace Engine
{
    public class AnyConditionNode: ConditionNode
    {
        public override List<long> GetRowsThatSatisfy(Table table, int limit)
        {
            return table.FirstColumn.AllIndices(limit);
        }
    }
}
