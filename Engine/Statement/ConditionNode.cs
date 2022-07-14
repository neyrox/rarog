using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public abstract class ConditionNode
    {
        public abstract List<long> GetRowsThatSatisfy(Table table, IStorage storage);
    }
}
