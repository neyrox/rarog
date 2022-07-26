namespace Engine
{
    public abstract class ResultColumnBase<T>: ResultColumn
    {
        public readonly T[] Values;

        public override int Count { get { return Values.Length; } }

        public T this[int idx] { get { return Values[idx]; } }

        protected ResultColumnBase(string name, T[] vals)
            : base(name)
        {
            Values = vals;
        }
        
        public override string Get(int index)
        {
            if (index >= Values.Length)
                return string.Empty;

            return Values[index].ToString();
        }
    }
}
