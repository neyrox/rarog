using System;
using Common;

namespace Engine
{
    public class Shell
    {
        private static readonly Log Log = LogManager.Create<Shell>();

        private readonly Database db;

        public Shell(Database database)
        {
            db = database;
        }

        public Result Execute(string query)
        {
            Log.Debug(query);
            try
            {
                var tokens = Lexer.Split(query);
                var command = Parser.Convert(tokens);

                // TODO: replace with empty command
                if (command == null)
                    throw new Exception("wrong query");

                return command.Execute(db);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return new Result(null, e.Message);
            }
        }
    }
}
