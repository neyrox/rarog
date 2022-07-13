using System;
using System.Collections.Generic;
using Engine.Serialization;

namespace Engine.Storage
{
    public class IntPage
    {
        public static SortedDictionary<long, int> Load(PageHeader header, byte[] buffer)
        {
            var result = new SortedDictionary<long, int>();
            int offset = PageHeader.DataOffset;
            // TODO: compress
            for (int i = 0; i < header.Count; ++i)
            {
                var idx = BytePacker.UnpackSInt64(buffer, ref offset);
                int val = BytePacker.UnpackSInt32(buffer, ref offset);
                result.Add(idx, val);
            }

            return result;
        }

        public static byte[] Serialize(SortedDictionary<long, int> idxVals)
        {
            // TODO: split pages
            var buffer = new byte[PageDesc.Size];
            long minIdx = Int64.MaxValue;
            long maxIdx = Int64.MinValue;
            int offset = PageHeader.DataOffset;
            foreach (var idxVal in idxVals)
            {
                if (idxVal.Key < minIdx)
                    minIdx = idxVal.Key;
                if (idxVal.Key > maxIdx)
                    maxIdx = idxVal.Key;

                BytePacker.PackSInt64(buffer, idxVal.Key, ref offset);
                BytePacker.PackSInt32(buffer, idxVal.Value, ref offset);
            }
            var header = new PageHeader(minIdx, maxIdx, idxVals.Count);
            header.Serialize(buffer);

            return buffer;
        }
    }
}