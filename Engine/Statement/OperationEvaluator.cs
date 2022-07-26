using System;

namespace Engine
{
    public static class OperationEvaluator
    {
        public static ResultColumnInteger Eval(string op, ResultColumnInteger left, ResultColumnInteger right)
        {
            switch (op)
            {
                case "+":
                    return Add(left, right);
                case "-":
                    return Sub(left, right);
                case "*":
                    return Mul(left, right);
                case "/":
                    return Div(left, right);
                default:
                    throw new Exception($"Unknown operation {op}");
            }
        }
        
        public static ResultColumnBigInt Eval(string op, ResultColumnBigInt left, ResultColumnBigInt right)
        {
            switch (op)
            {
                case "+":
                    return Add(left, right);
                case "-":
                    return Sub(left, right);
                case "*":
                    return Mul(left, right);
                case "/":
                    return Div(left, right);
                default:
                    throw new Exception($"Unknown operation {op}");
            }
        }
        
        public static ResultColumnDouble Eval(string op, ResultColumnDouble left, ResultColumnDouble right)
        {
            switch (op)
            {
                case "+":
                    return Add(left, right);
                case "-":
                    return Sub(left, right);
                case "*":
                    return Mul(left, right);
                case "/":
                    return Div(left, right);
                default:
                    throw new Exception($"Unknown operation {op}");
            }
        }

        public static ResultColumnString Eval(string op, ResultColumnString left, ResultColumnString right)
        {
            switch (op)
            {
                case "+":
                    return Add(left, right);
                default:
                    throw new Exception($"Unknown operation {op} for strings");
            }
        }

        private static ResultColumnInteger Add(ResultColumnInteger left, ResultColumnInteger right)
        {
            var data = new int[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] + right[i];

            return new ResultColumnInteger(string.Empty, data);
        }

        private static ResultColumnBigInt Add(ResultColumnBigInt left, ResultColumnBigInt right)
        {
            var data = new long[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] + right[i];

            return new ResultColumnBigInt(string.Empty, data);
        }
        
        private static ResultColumnDouble Add(ResultColumnDouble left, ResultColumnDouble right)
        {
            var data = new double[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] + right[i];

            return new ResultColumnDouble(string.Empty, data);
        }

        private static ResultColumnString Add(ResultColumnString left, ResultColumnString right)
        {
            var data = new string[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] + right[i];

            return new ResultColumnString(string.Empty, data);
        }
        
        private static ResultColumnInteger Sub(ResultColumnInteger left, ResultColumnInteger right)
        {
            var data = new int[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] - right[i];

            return new ResultColumnInteger(string.Empty, data);
        }

        private static ResultColumnBigInt Sub(ResultColumnBigInt left, ResultColumnBigInt right)
        {
            var data = new long[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] - right[i];

            return new ResultColumnBigInt(string.Empty, data);
        }
        
        private static ResultColumnDouble Sub(ResultColumnDouble left, ResultColumnDouble right)
        {
            var data = new double[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] - right[i];

            return new ResultColumnDouble(string.Empty, data);
        }
        
        private static ResultColumnInteger Mul(ResultColumnInteger left, ResultColumnInteger right)
        {
            var data = new int[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] * right[i];

            return new ResultColumnInteger(string.Empty, data);
        }

        private static ResultColumnBigInt Mul(ResultColumnBigInt left, ResultColumnBigInt right)
        {
            var data = new long[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] * right[i];

            return new ResultColumnBigInt(string.Empty, data);
        }

        private static ResultColumnDouble Mul(ResultColumnDouble left, ResultColumnDouble right)
        {
            var data = new double[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] * right[i];

            return new ResultColumnDouble(string.Empty, data);
        }
        
        private static ResultColumnInteger Div(ResultColumnInteger left, ResultColumnInteger right)
        {
            var data = new int[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] / right[i];

            return new ResultColumnInteger(string.Empty, data);
        }

        private static ResultColumnBigInt Div(ResultColumnBigInt left, ResultColumnBigInt right)
        {
            var data = new long[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] / right[i];

            return new ResultColumnBigInt(string.Empty, data);
        }

        private static ResultColumnDouble Div(ResultColumnDouble left, ResultColumnDouble right)
        {
            var data = new double[left.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = left[i] / right[i];

            return new ResultColumnDouble(string.Empty, data);
        }
    }
}
