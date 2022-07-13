using System;
using Engine.Serialization;

namespace Engine.Storage
{
    public enum PageTypes : byte
    {
        Empty = 0,
        IntColumn = 1,
        DblColumn = 2,
        StrColumn = 3,
    }

    public class PageHeader
    {
        public const int DataOffset = sizeof(byte) + sizeof(long) * 2 + sizeof(int);

        public readonly long MinIdx;
        public readonly long MaxIdx;
        public readonly int Count;

        private byte flags;
        /*
        private const byte RangeClosedLeftBit = 0x1;
        private const byte RangeClosedRightBit = 0x2;

        public bool RangeClosedLeft
        {
            get => GetFlag(RangeClosedLeftBit);
            set => SetFlag(value, RangeClosedLeftBit);
        }
        public bool RangeClosedRight
        {
            get => GetFlag(RangeClosedRightBit);
            set => SetFlag(value, RangeClosedRightBit);
        }*/

        public PageHeader(long minIdx, long maxIdx, int count)
        {
            MinIdx = minIdx;
            MaxIdx = maxIdx;
            Count = count;
        }

        public PageHeader(byte[] buffer)
        {
            flags = buffer[0];
            int offset = 1;
            MinIdx = BytePacker.UnpackSInt64(buffer, ref offset);
            MaxIdx = BytePacker.UnpackSInt64(buffer, ref offset);
            Count = BytePacker.UnpackSInt32(buffer, ref offset);
        }

        public void Serialize(byte[] buffer)
        {
            buffer[0] = flags;
            int offset = 1;
            BytePacker.PackSInt64(buffer, MinIdx, ref offset);
            BytePacker.PackSInt64(buffer, MaxIdx, ref offset);
            BytePacker.PackSInt32(buffer, Count, ref offset);
        }

        public bool Include(long idx)
        {
            return idx >= MinIdx && idx <= MaxIdx;
        }

        private bool GetFlag(byte bit)
        {
            return (flags & bit) != 0;
        }

        private void SetFlag(bool value, byte bit)
        {
            if (value)
                flags |= bit;
            else
                flags = (byte) (flags & ~bit);
        }
    }
}
