using System.Collections.Generic;

namespace Engine
{
    public abstract class ConditionNode
    {
        public abstract List<long> GetRowsThatSatisfy(Table table);
    }
}
