using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Engine.Serialization;

namespace Engine.Storage
{
    public static class StreamStorage
    {
        private const int FormatVersion = 0;

        public static Column LoadColumn(Stream stream)
        {
            // TODO: group fixed and variable-length items?
            var buf = new byte[1024];
            stream.Read(buf, 0, sizeof(int) + sizeof(byte));
            var formatVersion = BitConverter.ToInt32(buf, 0);
            if (formatVersion != FormatVersion)
                return null;

            var nameLen = buf[sizeof(int)];
            stream.Read(buf, 0, nameLen);
            var name = Encoding.UTF8.GetString(buf, 0, nameLen);

            stream.Read(buf, 0, sizeof(byte));
            var typeLen = buf[0];
            stream.Read(buf, 0, typeLen);
            var columnType = Encoding.UTF8.GetString(buf, 0, typeLen);

            Console.WriteLine($"Loading column {name} of type {columnType}");

            switch (columnType)
            {
                case "Int":
                    return LoadIntegerColumn(name, stream);
                case "Dbl":
                    return LoadDoubleColumn(name, stream);
                case "Str":
                    return LoadVarCharColumn(name, stream);
            }

            return null;
        }

        public static void Store(ColumnInteger column, Stream stream)
        {
            Write(stream, BitConverter.GetBytes(FormatVersion));
            BytePacker.PackString8(stream, column.Name);

            BytePacker.PackString8(stream, "Int");
            Write(stream, BitConverter.GetBytes(column.Count));
            foreach (var iv in column.IdxValues)
            {
                Write(stream, BitConverter.GetBytes(iv.Key));
                Write(stream, BitConverter.GetBytes(iv.Value));
            }
        }

        public static void Store(ColumnDouble column, Stream stream)
        {
            Write(stream, BitConverter.GetBytes(FormatVersion));
            BytePacker.PackString8(stream, column.Name);

            BytePacker.PackString8(stream, "Dbl");
            Write(stream, BitConverter.GetBytes(column.Count));
            foreach (var iv in column.IdxValues)
            {
                Write(stream, BitConverter.GetBytes(iv.Key));
                Write(stream, BitConverter.GetBytes(iv.Value));
            }
        }

        public static void Store(ColumnVarChar column, Stream stream)
        {
            Write(stream, BitConverter.GetBytes(FormatVersion));
            BytePacker.PackString8(stream, column.Name);

            BytePacker.PackString8(stream, "Str");
            Write(stream, BitConverter.GetBytes(column.MaxLength));
            Write(stream, BitConverter.GetBytes(column.Count));
            foreach (var iv in column.IdxValues)
            {
                Write(stream, BitConverter.GetBytes(iv.Key));
                var bytes = Encoding.UTF8.GetBytes(iv.Value);
                Write(stream, BitConverter.GetBytes(bytes.Length));
                Write(stream, bytes);
            }
        }

        private static Column LoadIntegerColumn(string name, Stream stream)
        {
            var idxValues = new SortedDictionary<long, int>();
            var buf = new byte[sizeof(long) + sizeof(int)];
            stream.Read(buf, 0, sizeof(int));
            var count = BitConverter.ToInt32(buf, 0);
            // TODO: batch reads
            for (int i = 0; i < count; ++i)
            {
                stream.Read(buf, 0, sizeof(long) + sizeof(int));
                var idx = BitConverter.ToInt64(buf, 0);
                var val = BitConverter.ToInt32(buf, sizeof(long));
                idxValues.Add(idx, val);
            }

            return new ColumnInteger(name, idxValues);
        }

        private static Column LoadDoubleColumn(string name, Stream stream)
        {
            var idxValues = new SortedDictionary<long, double>();
            var buf = new byte[sizeof(long) + sizeof(double)];
            stream.Read(buf, 0, sizeof(int));
            var count = BitConverter.ToInt32(buf, 0);
            // TODO: batch reads
            for (int i = 0; i < count; ++i)
            {
                stream.Read(buf, 0, sizeof(long) + sizeof(double));
                var idx = BitConverter.ToInt64(buf, 0);
                var val = BitConverter.ToDouble(buf, sizeof(long));
                idxValues.Add(idx, val);
            }

            return new ColumnDouble(name, idxValues);
        }

        private static Column LoadVarCharColumn(string name, Stream stream)
        {
            var idxValues = new SortedDictionary<long, string>();
            var buf = new byte[65536];
            stream.Read(buf, 0, sizeof(int) + sizeof(int));
            var maxLength = BitConverter.ToInt32(buf, 0);
            var count = BitConverter.ToInt32(buf, sizeof(int));
            // TODO: batch reads
            for (int i = 0; i < count; ++i)
            {
                stream.Read(buf, 0, sizeof(long) + sizeof(int));
                var idx = BitConverter.ToInt64(buf, 0);
                var len = BitConverter.ToInt32(buf, sizeof(long));
                if (len > buf.Length)
                {
                    buf = new byte[Math.Max(len + 2, buf.Length * 2)];
                }
                stream.Read(buf, 0, len);
                var val = Encoding.UTF8.GetString(buf, 0, len);
                idxValues.Add(idx, val);
            }

            return new ColumnVarChar(name, idxValues);
        }

        private static void Write(Stream fs, byte[] buf)
        {
            fs.Write(buf, 0, buf.Length);
        }
    }
}
