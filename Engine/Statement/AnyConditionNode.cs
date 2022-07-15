using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public class AnyConditionNode: ConditionNode
    {
        public override List<long> GetRowsThatSatisfy(Table table, IStorage storage, int limit)
        {
            return table.FirstColumn.AllIndices(storage, limit);
        }
    }
}
