using System.Collections.Generic;

namespace Engine
{
    public abstract class ResultColumnBase<T>: ResultColumn
    {
        protected readonly T[] values;

        public override int Count { get { return values.Length; } }

        protected ResultColumnBase(T[] vals)
        {
            values = vals;
        }
    }
}
