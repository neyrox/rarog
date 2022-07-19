using System;
using Engine;
using Rarog;

namespace Bench
{
    public abstract class JobBase
    {
        protected Connection dbConn;
        protected Options options;
        protected Random rnd;
        protected string name;

        protected JobBase(Options options, string name)
        {
            this.options = options;
            this.name = name;

            int port = 33777;
            dbConn = new Connection("localhost", port);
            Console.WriteLine($"{name} Connected to database");

            rnd = new Random(DateTime.UtcNow.ToFileTimeUtc().GetHashCode());
        }

        public void Run()
        {
            try
            {
                RunInternal();
            }
            finally
            {
                dbConn.Close();
            }
        }

        protected abstract void RunInternal();

        protected Result Perform(string query)
        {
            var result = dbConn.Perform(query);
            if (!result.IsOK)
                Console.WriteLine(result.Error);
            return result;
        }
    }
}
