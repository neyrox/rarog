using System.Collections.Generic;

namespace Engine
{
    public abstract class ResultColumnBase
    {
        public abstract int Count { get; }

        public abstract string Get(int index);

        public List<string> All()
        {
            var result = new List<string>(Count);

            for (int i = 0; i < Count; ++i)
            {
                result.Add(Get(i));
            }

            return result;
        }
    }
}
