namespace Engine
{
    public abstract class ResultColumnBase<T>: ResultColumn
    {
        protected readonly T[] values;

        public override int Count { get { return values.Length; } }

        public T this[int idx] { get { return values[idx]; } }

        protected ResultColumnBase(T[] vals)
        {
            values = vals;
        }
    }
}
