using System;

namespace Engine
{
    public class Shell
    {
        private readonly Database db;

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

                return command.Execute(db);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Result(null, e.Message);
            }
        }
    }
}
