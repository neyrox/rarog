namespace Engine
{
    public interface IResultFactory<T>
    {
        ResultColumn Create(string name, T[] vals);
    }

    public class IntResultFactory : IResultFactory<int>
    {
        public ResultColumn Create(string name, int[] vals)
        {
            return new ResultColumnInteger(name, vals);
        }
    }
    
    public class BigIntResultFactory : IResultFactory<long>
    {
        public ResultColumn Create(string name, long[] vals)
        {
            return new ResultColumnBigInt(name, vals);
        }
    }
    
    public class DoubleResultFactory : IResultFactory<double>
    {
        public ResultColumn Create(string name, double[] vals)
        {
            return new ResultColumnDouble(name, vals);
        }
    }

    public class StringResultFactory : IResultFactory<string>
    {
        public ResultColumn Create(string name, string[] vals)
        {
            return new ResultColumnString(name, vals);
        }
    }
}