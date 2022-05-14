using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    public static class BytePacker
    {
        public static void PackUInt8(byte[] buffer, byte val, ref int offset)
        {
            buffer[offset++] = val;
        }

        public static byte UnpackUInt8(byte[] buffer, ref int offset)
        {
            var val = buffer[offset++];
            return val;
        }

        public static void PackUInt16(byte[] buffer, ushort val, ref int offset)
        {
            buffer[offset++] = (byte)(val);
            buffer[offset++] = (byte)(val >> 8);
        }

        public static ushort UnpackUInt16(byte[] buffer, ref int offset)
        {
            ushort val = buffer[offset++];
            val |= (ushort)(buffer[offset++] << 8);
            return val;
        }

        public static void PackUInt32(byte[] buffer, uint val, ref int offset)
        {
            buffer[offset++] = (byte)(val);
            buffer[offset++] = (byte)(val >> 8);
            buffer[offset++] = (byte)(val >> 16);
            buffer[offset++] = (byte)(val >> 24);
        }

        public static uint UnpackUInt32(byte[] buffer, ref int offset)
        {
            uint val = buffer[offset++];
            val |= (uint)buffer[offset++] << 8;
            val |= (uint)buffer[offset++] << 16;
            val |= (uint)buffer[offset++] << 24;
            return val;
        }

        public static void PackUInt64(byte[] buffer, ulong val, ref int offset)
        {
            buffer[offset++] = (byte)(val);
            buffer[offset++] = (byte)(val >> 8);
            buffer[offset++] = (byte)(val >> 16);
            buffer[offset++] = (byte)(val >> 24);
            buffer[offset++] = (byte)(val >> 32);
            buffer[offset++] = (byte)(val >> 40);
            buffer[offset++] = (byte)(val >> 48);
            buffer[offset++] = (byte)(val >> 56);
        }

        public static ulong UnpackUInt64(byte[] buffer, ref int offset)
        {
            ulong val = buffer[offset++];
            val |= (ulong)buffer[offset++] << 8;
            val |= (ulong)buffer[offset++] << 16;
            val |= (ulong)buffer[offset++] << 24;
            val |= (ulong)buffer[offset++] << 32;
            val |= (ulong)buffer[offset++] << 40;
            val |= (ulong)buffer[offset++] << 48;
            val |= (ulong)buffer[offset++] << 56;
            return val;
        }

        public static void PackSInt32(byte[] buffer, int val, ref int offset)
        {
            uint uintValue;
            unsafe
            {
                uintValue = *((uint*)(&val));
            }
            PackUInt32(buffer, uintValue, ref offset);
        }

        public static int UnpackSInt32(byte[] buffer, ref int offset)
        {
            uint uintValue = UnpackUInt32(buffer, ref offset);

            int intValue;
            unsafe
            {
                intValue = *((int*)(&uintValue));
            }
            return intValue;
        }

        public static void PackDouble(byte[] buffer, double val, ref int offset)
        {
            ulong longValue;
            unsafe
            {
                longValue = *((ulong*)(&val));
            }
            PackUInt64(buffer, longValue, ref offset);
        }

        public static double UnpackDouble(byte[] buffer, ref int offset)
        {
            ulong ulongValue = UnpackUInt64(buffer, ref offset);

            double doubleValue;
            unsafe
            {
                doubleValue = *((double*)(&ulongValue));
            }
            return doubleValue;
        }

        public static void PackString8(byte[] buffer, string val, ref int offset)
        {
            int len = 0;
            if (val != null)
            {
                len = Encoding.UTF8.GetBytes(val, 0, val.Length, buffer, offset + 1);
                if (len >= 256)
                    throw new ArgumentOutOfRangeException(nameof(val), "Can't pack string with more than 255 bytes");
            }

            buffer[offset++] = (byte)len;
            offset += len;
        }

        public static void PackString16(byte[] buffer, string val, ref int offset)
        {
            int len = 0;
            if (val != null)
            {
                len = Encoding.UTF8.GetBytes(val, 0, val.Length, buffer, offset + 2);
                if (len >= 65536)
                    throw new ArgumentOutOfRangeException(nameof(val), "Can't pack string with more than 65535 bytes");
            }

            buffer[offset++] = (byte)(len);
            buffer[offset++] = (byte)(len >> 8);
            offset += len;
        }

        public static string UnpackString16(byte[] buffer, ref int offset)
        {
            int len = buffer[offset++];
            len += (buffer[offset++] << 8);
            if (len == 0)
                return null;

            var val = Encoding.UTF8.GetString(buffer, offset, len);
            offset += len;
            return val;
        }

        public static string UnpackString8(byte[] buffer, ref int offset)
        {
            int len = buffer[offset++];
            if (len == 0)
                return null;

            var val = Encoding.UTF8.GetString(buffer, offset, len);
            offset += len;
            return val;
        }
    }
}
