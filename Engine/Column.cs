using System.Collections.Generic;

namespace Engine
{
    public abstract class Column
    {
        public abstract void FullUpdate(string value);

        public abstract void Update(int row, string value);

        public abstract void Insert(string value);

        public abstract List<int> GetInclusions(string value);

        public abstract string Get(int index);
    }
}
