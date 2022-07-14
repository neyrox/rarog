using System;
using System.Threading;

namespace Engine
{
    public class Shell
    {
        private readonly Database db;
        private readonly object @lock = new object();
        private readonly TimeSpan lockTimeout = new TimeSpan(0, 1, 0);

        public Shell(Database database)
        {
            db = database;
            db.Load();
        }

        public Result Execute(string query)
        {
            try
            {
                var tokens = Lexer.Split(query);
                var command = Parser.Convert(tokens);

                // TODO: replace with empty command
                if (command == null)
                    throw new Exception("wrong query");

                if (Monitor.TryEnter(@lock, lockTimeout))
                {
                    try
                    {
                        return command.Execute(db);
                    }
                    finally
                    {
                        Monitor.Exit(@lock);
                    }
                }
                throw new Exception("failed to get database lock");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Result(null, e.Message);
            }
        }
    }
}
