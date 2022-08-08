using System;
using Engine.Serialization;

namespace Engine.Storage
{
    public class PageHeader
    {
        public const int DataOffset = Page.HeaderSize;

        private long minIdx;
        private long maxIdx;
        private int count;

        public long MinIdx
        {
            get => minIdx;
            set
            {
                minIdx = value;
            }
        }

        public long MaxIdx
        {
            get => maxIdx;
            set
            {
                maxIdx = value;  
            }
        }

        public int Count
        {
            get => count;
            set
            {
                if (value < 0)
                    throw new Exception("Wrong header");
                
                if (value > MaxIdx - MinIdx + 1)
                    throw new Exception("Wrong header");

                count = value;  
            }
        }

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
