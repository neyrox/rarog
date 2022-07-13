using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public abstract class ConditionNode
    {
        public abstract List<long> GetRowsThatSatisfy(Table table, IStorage storage);
    }
    
    public abstract class IdxIntPredicate
    {
        public abstract bool Satisfy(long idx, int val);
    }

    public abstract class UpdateIntPredicate
    {
        public abstract bool Satisfy(long idx, int val);
        public abstract int Process(long idx, int val);
    }

    public class UpdateIntToOtherInt : UpdateIntPredicate
    {
        private int from;
        private int to;

        public UpdateIntToOtherInt(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Satisfy(long idx, int val)
        {
            return val == from;
        }

        public override int Process(long idx, int val)
        {
            return to;
        }
    }

    public class SomeIndicesInts : IdxIntPredicate
    {
        private readonly ISet<long> indices;

        public SomeIndicesInts(ISet<long> indices)
        {
            this.indices = indices;
        }

        public override bool Satisfy(long idx, int val)
        {
            return indices.Contains(idx);
        }
    }
}
