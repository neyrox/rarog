﻿using System;

namespace Engine
{
    public class Shell
    {
        private Database db;
        private object mutex = new object();

        public Shell(Database database)
        {
            db = database;
        }

        public Result Execute(string query)
        {
            try
            {
                var tokens = Lexer.Split(query);
                var command = Parser.Convert(tokens);

                // TODO: replace with empty command
                if (command != null)
                {
                    lock (mutex)
                    {
                        return command.Execute(db);
                    }
                }

                throw new Exception("wrong query");
            }
            catch (Exception e)
            {
                return new Result(null, e.Message);
            }
        }
    }
}
