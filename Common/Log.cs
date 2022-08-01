using System;
using System.Diagnostics;
using System.Threading;

namespace Common
{
    public class Log
    {
        public static volatile int ErrorsCount;

        private readonly string _name;

        public Log(string name)
        {
            _name = name;
        }

        [Conditional("DEBUG")]
        public void Debug(string msg)
        {
            Console.WriteLine($"{DateTime.UtcNow:s}|DBG|{_name}|{msg}");
        }

        public void Info(string msg)
        {
            Console.WriteLine($"{DateTime.UtcNow:s}|INFO|{_name}|{msg}");
        }

        public void Warn(string msg)
        {
            Console.WriteLine($"{DateTime.UtcNow:s}|WARN|{_name}|{msg}");
        }

        public void Error(string msg)
        {
            Interlocked.Increment(ref ErrorsCount);
            Console.WriteLine($"{DateTime.UtcNow:s}|ERROR|{_name}|{msg}");
        }

        public void Error(Exception e)
        {
            string msg = e.Message;
            try
            {
                msg = e.ToString();
            }
            catch (Exception)
            {
            }

            Error(msg);
        }
    }
}
