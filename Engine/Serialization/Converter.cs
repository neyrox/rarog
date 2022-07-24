namespace Engine.Storage
{
    public interface IConverter<T>
    {
        T FromString(string str, int length = 0);
    }

    public class IntConverter : IConverter<int>
    {
        public static IConverter<int> Instance = new IntConverter();
        
        public int FromString(string str, int length)
        {
            return int.Parse(str);
        }
    }
    
    public class BigIntConverter : IConverter<long>
    {
        public static IConverter<long> Instance = new BigIntConverter();

        public long FromString(string str, int length)
        {
            return long.Parse(str);
        }
    }

    public class DoubleConverter : IConverter<double>
    {
        public static IConverter<double> Instance = new DoubleConverter();

        public double FromString(string str, int length)
        {
            return double.Parse(str);
        }
    }

    public class VarCharConverter : IConverter<string>
    {
        public static IConverter<string> Instance = new VarCharConverter();

        public string FromString(string str, int length)
        {
            if (str == null)
                return string.Empty;

            if (length > 0 && length < str.Length)
                return str.Substring(0, length);

            return str;
        }
    }
}
