using System.Collections.Generic;

namespace Engine.Storage
{
    public struct PageIdxCondition
    {
        private SortedSet<long> indices;

        public bool All => indices == null;

        public PageIdxCondition(SortedSet<long> indices)
        {
            this.indices = indices;
        }

        public bool Satisfy(PageHeader header)
        {
            if (indices == null)
                return true;

            return Overlap(header.MinIdx, header.MaxIdx, indices.Min, indices.Max);
        }

        public static bool Overlap(long start1, long end1, long start2, long end2)
        {
            return end1 >= start2 && end2 >= start1;
        }
    }

    public struct PageDesc
    {
        public const int Size = 512 * 1024;  // SSD block size
    }

    public struct Page<T>
    {
        public SortedDictionary<long, T> IdxVals;
        public SortedSet<long> Deleted;

        public static Page<T> Empty => new Page<T> {IdxVals = new SortedDictionary<long, T>(), Deleted = new SortedSet<long>()};
    }
}
