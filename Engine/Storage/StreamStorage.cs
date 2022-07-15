using System;
using System.IO;
using Engine.Serialization;

namespace Engine.Storage
{
    public static class StreamStorage
    {
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

        public static Column LoadColumnMeta(Stream stream, string tablePath)
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

            Console.WriteLine($"Loading column {name} of type {columnType}");

            switch (columnType)
            {
                case "Int":
                    return new ColumnInteger(tablePath, name);
                case "Dbl":
                    return new ColumnDouble(tablePath, name);
                case "Str":
                    return new ColumnVarChar(tablePath, name);
            }

            return null;
        }

        public static byte[] NextPage(Stream stream)
        {
            var buf = new byte[Page.Size];
            stream.Read(buf, 0, buf.Length);
            return buf;
        }

        public static bool FindPage(Stream stream, long idx, out PageHeader header, out byte[] page)
        {
            header = null;
            page = null;

            while (stream.Position < stream.Length)
            {
                page = NextPage(stream);
                header = new PageHeader(page);
                if (header.Include(idx))
                    return true;
            }

            return false;
        }

        public static bool FindPageForInsert(Stream stream, long idx, out PageHeader header, out byte[] page)
        {
            header = null;
            page = null;

            if (stream.Length < Page.Size)
                return false;

            // Always last
            // TODO: rework?
            stream.Seek(-Page.Size, SeekOrigin.End);
            page = NextPage(stream);
            header = new PageHeader(page);

            return true;
        }

        public static void Write(Stream fs, byte[] buf)
        {
            fs.Write(buf, 0, buf.Length);
        }

        public static void WriteBack(Stream stream, byte[] page)
        {
            stream.Seek(-Page.Size, SeekOrigin.Current);
            stream.Write(page, 0, page.Length);
        }

        public static bool Overlap(long start1, long end1, long start2, long end2)
        {
            return end1 >= start2 && end2 >= start1;
        }
    }
}
