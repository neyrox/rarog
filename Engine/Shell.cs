using System;
using System.Collections.Generic;

namespace Engine
{
    public class Shell
    {
        private Database db;

        public Shell(Database database)
        {
            db = database;
        }

        public List<List<string>> Execute(string query)
        {
            var tokens = Lexer.Split(query);
            var command = Parser.Convert(tokens);
            var updateCmd = command as UpdateNode;
            var selectCmd = command as SelectNode;
            var insertCmd = command as InsertNode;
            var createTableCmd = command as CreateTableNode;
            if (updateCmd != null)
            {
                db.Execute(updateCmd);
            }
            else if (selectCmd != null)
            {
                return db.Execute(selectCmd);
            }
            else if (insertCmd != null)
            {
                db.Execute(insertCmd);
            }
            else if (createTableCmd != null)
            {
                db.Execute(createTableCmd);
            }
            else
                throw new Exception("wrong query");

            return null;
        }
    }
}
