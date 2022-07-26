using System;

namespace Engine
{
    public class BinaryOpVisitor : IResultColumnVisitor
    {
        private ResultColumnDouble dblClmn = null;
        private ResultColumnInteger intClmn = null;
        private ResultColumnBigInt lngClmn = null;
        private ResultColumnString strClmn = null;

        private ResultColumn result = null;

        public string Op;

        public ResultColumn ExtractResult()
        {
            var res = result;
            result = null;
            return res;
        }
        
        public void Visit(ResultColumnDouble column)
        {
            if (dblClmn != null)
            {
                result = OperationEvaluator.Eval(Op, dblClmn, column);
                dblClmn = null;
            }
            else if (intClmn != null)
            {
                result = OperationEvaluator.Eval(Op, ToDouble(intClmn), column);
                intClmn = null;
            }
            else if (lngClmn != null)
            {
                result = OperationEvaluator.Eval(Op, ToDouble(lngClmn), column);
                lngClmn = null;
            }
            else if (strClmn != null)
            {
                result = OperationEvaluator.Eval(Op, strClmn, ToString(column));
                strClmn = null;
            }
            else
            {
                dblClmn = column;
            }
        }

        public void Visit(ResultColumnInteger column)
        {
            if (intClmn != null)
            {
                result = OperationEvaluator.Eval(Op, intClmn, column);
                intClmn = null;
            }
            else if (dblClmn != null)
            {
                result = OperationEvaluator.Eval(Op, dblClmn, ToDouble(column));
                dblClmn = null;
            }
            else if (lngClmn != null)
            {
                result = OperationEvaluator.Eval(Op, lngClmn, ToBigInt(column));
                lngClmn = null;
            }
            else if (strClmn != null)
            {
                result = OperationEvaluator.Eval(Op, strClmn, ToString(column));
                strClmn = null;
            }
            else
            {
                intClmn = column;
            }

        }

        public void Visit(ResultColumnBigInt column)
        {
        }

        public void Visit(ResultColumnString column)
        {
        }

        private static ResultColumnString ToString<T>(ResultColumnBase<T> col)
        {
            var data = new string[col.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = col[i].ToString();

            return new ResultColumnString(string.Empty, data);
        }

        private static ResultColumnDouble ToDouble(ResultColumnInteger col)
        {
            var data = new double[col.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = col[i];

            return new ResultColumnDouble(string.Empty, data);
        }

        private static ResultColumnDouble ToDouble(ResultColumnBigInt col)
        {
            var data = new double[col.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = col[i];

            return new ResultColumnDouble(string.Empty, data);
        }

        private static ResultColumnBigInt ToBigInt(ResultColumnInteger col)
        {
            var data = new long[col.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = col[i];

            return new ResultColumnBigInt(string.Empty, data);
        }
    }
}