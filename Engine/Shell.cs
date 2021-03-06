﻿using System;

namespace Engine
{
    public class Shell
    {
        private Database db;

        public Shell(Database database)
        {
            db = database;
        }

        public Result Execute(string query)
        {
            var tokens = Lexer.Split(query);
            var command = Parser.Convert(tokens);

            // TODO: replace with empty command
            if (command != null)
                return command.Execute(db);
            else
                throw new Exception("wrong query");
        }
    }
}
