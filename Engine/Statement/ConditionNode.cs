using System.Collections.Generic;

namespace Engine
{
    public abstract class ConditionNode
    {
        public abstract List<int> GetRowsThatSatisfy(Table table);
    }
}
