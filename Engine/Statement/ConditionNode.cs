using System.Collections.Generic;

namespace Engine
{
    public abstract class ConditionNode: Node
    {
        public abstract List<int> GetRowsThatSatisfy(Table table);
    }
}
