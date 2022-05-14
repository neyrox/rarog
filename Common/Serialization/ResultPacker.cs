using System.Collections.Generic;

namespace Engine.Serialization
{
    public class ResultPacker : IResultColumnVisitor
    {
        byte[] _buffer;
        int _offset;

        private enum ColumnType : byte
        {
            ColumnInteger = 1,
            ColumnDouble = 2,
            ColumnString = 3
        }

        public byte[] PackResult(Result res, out int length)
        {
            _buffer = new byte[1024 * 1024];
            _offset = 0;
            // Space for packer length
            BytePacker.PackSInt32(_buffer, 0, ref _offset);
            BytePacker.PackString16(_buffer, res.Error, ref _offset);
            if (res.Columns == null)
            {
                BytePacker.PackSInt32(_buffer, 0, ref _offset);
            }
            else
            {
                BytePacker.PackSInt32(_buffer, res.Columns.Count, ref _offset);
                for (int i = 0; i < res.Columns.Count; ++i)
                {
                    var column = res.Columns[i];
                    BytePacker.PackString8(_buffer, column.Name, ref _offset);
                    BytePacker.PackSInt32(_buffer, column.Count, ref _offset);
                    column.Accept(this);
                }
            }
            length = _offset;
            _offset = 0;
            BytePacker.PackSInt32(_buffer, length, ref _offset);

            return _buffer;
        }

        public Result UnpackResult(byte[] buffer, int start)
        {
            int offset = start;
            var length = BytePacker.UnpackSInt32(buffer, ref offset);
            var error = BytePacker.UnpackString16(buffer, ref offset);
            var columns = new List<ResultColumn>();
            var columnsCount = BytePacker.UnpackSInt32(buffer, ref offset);
            for (int i = 0; i < columnsCount; ++i)
            {
                var column = UnpackColumn(buffer, ref offset);
                if (column != null)
                    columns.Add(column);
            }

            var res = new Result(columns, error);
            return res;
        }

        public ResultColumn UnpackColumn(byte[] buffer, ref int offset)
        {
            var name = BytePacker.UnpackString8(buffer, ref offset);
            var itemsCount = BytePacker.UnpackSInt32(buffer, ref offset);
            var columnType = (ColumnType)BytePacker.UnpackUInt8(buffer, ref offset);
            switch (columnType)
            {
                case ColumnType.ColumnInteger:
                {
                    int[] items = new int[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = BytePacker.UnpackSInt32(buffer, ref offset);
                    }
                    return new ResultColumnInteger(name, items);
                }
                case ColumnType.ColumnDouble:
                {
                    double[] items = new double[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = BytePacker.UnpackDouble(buffer, ref offset);
                    }
                    return new ResultColumnDouble(name, items);
                }
                case ColumnType.ColumnString:
                {
                    string[] items = new string[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = BytePacker.UnpackString16(buffer, ref offset);
                    }
                    return new ResultColumnString(name, items);
                }
            }

            return null;
        }

        public void Visit(ResultColumnDouble column)
        {
            BytePacker.PackUInt8(_buffer, (byte)ColumnType.ColumnDouble, ref _offset);
            for (int i = 0; i < column.Count; ++i)
            {
                BytePacker.PackDouble(_buffer, column[i], ref _offset);
            }
        }

        public void Visit(ResultColumnInteger column)
        {
            BytePacker.PackUInt8(_buffer, (byte)ColumnType.ColumnInteger, ref _offset);
            for (int i = 0; i < column.Count; ++i)
            {
                BytePacker.PackSInt32(_buffer, column[i], ref _offset);
            }
        }

        public void Visit(ResultColumnString column)
        {
            BytePacker.PackUInt8(_buffer, (byte)ColumnType.ColumnString, ref _offset);
            for (int i = 0; i < column.Count; ++i)
            {
                BytePacker.PackString16(_buffer, column[i], ref _offset);
            }
        }
    }
}
