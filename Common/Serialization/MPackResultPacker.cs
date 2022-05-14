using System;
using System.Collections.Generic;
using System.IO;
using CS;

namespace Engine.Serialization
{
    public class MPackResultPacker : IResultColumnVisitor
    {
        MPackArray _arr;

        public byte[] PackResult(Result res)
        {
            var dict = new MPackMap();

            if (res.Error != null)
            {
                dict.Add("Error", MPack.From(res.Error));
            }
            else
            {
                _arr = new MPackArray();

                if (res.Columns != null)
                {
                    for (int i = 0; i < res.Columns.Count; ++i)
                    {
                        var column = res.Columns[i];
                        column.Accept(this);
                    }
                }
                
                dict.Add("Columns", _arr);
                _arr = null;
            }

            var buf = dict.EncodeToBytes();
            return buf;
        }

        public Result UnpackResult(byte[] buffer)
        {
            var rec = MPack.ParseFromBytes(buffer) as MPackMap;
            string error = null;
            var columns = new List<ResultColumn>();

            if (rec.ContainsKey("Error"))
            {
                error = rec["Error"].To<string>();
            }

            if (rec.ContainsKey("Columns"))
            {
                var srcCols = rec["Columns"] as MPackArray;
                for (int i = 0; i < srcCols.Count; ++i)
                {
                    var column = UnpackColumn(srcCols[i] as MPackMap);
                    if (column != null)
                        columns.Add(column);
                }
            }

            var res = new Result(columns, error);
            return res;
        }

        public ResultColumn UnpackColumn(MPackMap srcCol)
        {
            var columnName = srcCol["Name"].To<string>();
            var columnType = srcCol["Type"].To<string>();
            var columnVals = srcCol["Vals"] as MPackArray;
            var itemsCount = columnVals.Count;

            switch (columnType)
            {
                case "Int":
                {
                    int[] items = new int[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = columnVals[i].To<int>();
                    }
                    return new ResultColumnInteger(columnName, items);
                }
                case "Dbl":
                {
                    double[] items = new double[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = columnVals[i].To<double>();
                    }
                    return new ResultColumnDouble(columnName, items);
                }
                case "Str":
                {
                    string[] items = new string[itemsCount];
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        items[i] = columnVals[i].To<string>();
                    }
                    return new ResultColumnString(columnName, items);
                }
            }

            return null;
        }

        public void Visit(ResultColumnDouble column)
        {
            var vals = new MPackArray();
            for (int i = 0; i < column.Count; ++i)
            {
                vals.Add(MPack.From(column[i]));
            }

            var col = new MPackMap
            {
                {"Name", column.Name},
                {"Type", "Dbl"},
                {"Vals", vals}
            };

            _arr.Add(col);
        }

        public void Visit(ResultColumnInteger column)
        {
            var vals = new MPackArray();
            for (int i = 0; i < column.Count; ++i)
            {
                vals.Add(MPack.From(column[i]));
            }

            var col = new MPackMap
            {
                {"Name", column.Name},
                {"Type", "Int"},
                {"Vals", vals}
            };

            _arr.Add(col);
        }

        public void Visit(ResultColumnString column)
        {
            var vals = new MPackArray();
            for (int i = 0; i < column.Count; ++i)
            {
                vals.Add(MPack.From(column[i]));
            }

            var col = new MPackMap
            {
                {"Name", column.Name},
                {"Type", "Str"},
                {"Vals", vals}
            };

            _arr.Add(col);
        }
    }
}
