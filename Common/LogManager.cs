namespace Common
{
    public static class LogManager
    {
        public static Log Create<T>()
        {
            return Create(typeof(T).FullName);
        }

        public static Log Create(string name)
        {
            return new Log(name);
        }
    }
}
