using System;
using System.IO;
using Common;
using Engine.Serialization;

namespace Engine.Storage
{
    public static class StreamStorage
    {
        private static readonly Log Log = LogManager.Create(nameof(StreamStorage));

        private const int FormatVersion = 0;

        public static void StoreColumnMeta(Stream stream, Column column)
        {
            var buf = new byte[1024];
            int offset = 0;
            BytePacker.PackSInt32(buf, FormatVersion, ref offset);
            BytePacker.PackString8(buf, column.Name, ref offset);
            BytePacker.PackString8(buf, column.TypeNameP, ref offset);
            stream.Write(buf, 0, offset);
        }

        public static Column LoadColumnMeta(Stream stream, string tablePath, Registry registry)
        {
            // TODO: reformat
            var buf = new byte[1024];
            stream.Read(buf, 0, (int)Math.Min(stream.Length, buf.Length));
            int offset = 0;
            var formatVersion = BytePacker.UnpackSInt32(buf, ref offset);
            if (formatVersion != FormatVersion)
                return null;

            var name = BytePacker.UnpackString8(buf, ref offset);
            var columnType = BytePacker.UnpackString8(buf, ref offset);

            Log.Debug($"Loading column {name} of type {columnType}");

            switch (columnType)
            {
                case ResultColumnInteger.TypeTag:
                    return new ColumnBase<int>(tablePath, name, registry.IntTraits);
                case ResultColumnBigInt.TypeTag:
                    return new ColumnBase<long>(tablePath, name, registry.BigIntTraits);
                case ResultColumnDouble.TypeTag:
                    return new ColumnBase<double>(tablePath, name, registry.DoubleTraits);
                case ResultColumnString.TypeTag:
                    return new ColumnBase<string>(tablePath, name, registry.StrTraits);
            }

            return null;
        }

        public static void SeekToPage(Stream stream, int pageIdx)
        {
            try
            {
                stream.Seek(Page.Size * pageIdx, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void SeekToPageData(Stream stream, int pageIdx)
        {
            try
            {
                stream.Seek(Page.Size * pageIdx + Page.HeaderSize, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void SkipPage(Stream stream)
        {
            stream.Seek(Page.Size, SeekOrigin.Current);
        }

        public static void SkipPageData(Stream stream)
        {
            stream.Seek(Page.Size - Page.HeaderSize, SeekOrigin.Current);
        }

        public static byte[] ReadHeader(Stream stream)
        {
            var buf = new byte[Page.HeaderSize];
            stream.Read(buf, 0, buf.Length);
            return buf;
        }

        public static byte[] ReadPage(Stream stream)
        {
            var buf = new byte[Page.Size];
            stream.Read(buf, 0, buf.Length);
            return buf;
        }

        public static byte[] ReadPageData(Stream stream)
        {
            var buf = new byte[Page.Size - Page.HeaderSize];
            stream.Read(buf, 0, buf.Length);
            return buf;
        }

        public static void Write(Stream fs, byte[] buf)
        {
            fs.Write(buf, 0, buf.Length);
        }

        public static void Write(Stream stream, byte[] page, int pageIdx)
        {
            try
            {
                stream.Seek(Page.Size * pageIdx, SeekOrigin.Begin);
                stream.Write(page, 0, page.Length);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static bool Overlap(long start1, long end1, long start2, long end2)
        {
            return end1 >= start2 && end2 >= start1;
        }
    }
}
