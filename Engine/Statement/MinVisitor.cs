using System;

namespace Engine
{
    public class MinVisitor : IResultColumnVisitor
    {
        private ResultColumn result = null;

        public ResultColumn ExtractResult()
        {
            if (result == null)
                return new ResultColumnInteger("MIN", new int[0]);

            var res = result;
            result = null;
            return res;
        }

        public void Visit(ResultColumnDouble column)
        {
            if (column.Count == 0)
                return;

            var min = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val < min)
                    min = val;
            }

            result = new ResultColumnDouble("MIN", new [] {min});
        }

        public void Visit(ResultColumnInteger column)
        {
            if (column.Count == 0)
                return;

            var min = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val < min)
                    min = val;
            }

            result = new ResultColumnInteger("MIN", new [] {min});
        }

        public void Visit(ResultColumnBigInt column)
        {
            if (column.Count == 0)
                return;

            var min = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val < min)
                    min = val;
            }

            result = new ResultColumnBigInt("MIN", new [] {min});
        }

        public void Visit(ResultColumnString column)
        {
            if (column.Count == 0)
                return;

            var min = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (string.Compare(val,min, StringComparison.InvariantCulture) < 0)
                    min = val;
            }

            result = new ResultColumnString("MIN", new [] {min});
        }
    }
}