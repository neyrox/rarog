using System;

namespace Engine
{
    public class MaxVisitor : IResultColumnVisitor
    {
        private ResultColumn result = null;

        public ResultColumn ExtractResult()
        {
            if (result == null)
                return new ResultColumnInteger("MAX", new int[0]);

            var res = result;
            result = null;
            return res;
        }

        public void Visit(ResultColumnDouble column)
        {
            if (column.Count == 0)
                return;

            var max = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val > max)
                    max = val;
            }

            result = new ResultColumnDouble("MAX", new [] {max});
        }

        public void Visit(ResultColumnInteger column)
        {
            if (column.Count == 0)
                return;

            var max = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val > max)
                    max = val;
            }

            result = new ResultColumnInteger("MAX", new [] {max});
        }

        public void Visit(ResultColumnBigInt column)
        {
            if (column.Count == 0)
                return;

            var max = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (val > max)
                    max = val;
            }

            result = new ResultColumnBigInt("MAX", new [] {max});
        }

        public void Visit(ResultColumnString column)
        {
            if (column.Count == 0)
                return;

            var max = column.Values[0];

            for (int i = 1; i < column.Count; i++)
            {
                var val = column.Values[i];
                if (string.Compare(val,max, StringComparison.InvariantCulture) > 0)
                    max = val;
            }

            result = new ResultColumnString("MAX", new [] {max});
        }
    }
}