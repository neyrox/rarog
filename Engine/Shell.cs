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

        public Result Execute(string query)
        {
            // TODO: rework it
            var tokens = Lexer.Split(query);
            var command = Parser.Convert(tokens);
            var updateCmd = command as UpdateNode;
            var selectCmd = command as SelectNode;
            var insertCmd = command as InsertNode;
            var deleteCmd = command as DeleteNode;
            var createTableCmd = command as CreateTableNode;
            var dropTableCmd = command as DropTableNode;

            if (updateCmd != null)
            {
                return db.Execute(updateCmd);
            }
            else if (selectCmd != null)
            {
                return db.Execute(selectCmd);
            }
            else if (insertCmd != null)
            {
                return db.Execute(insertCmd);
            }
            else if (deleteCmd != null)
            {
                return db.Execute(deleteCmd);
            }
            else if (createTableCmd != null)
            {
                return db.Execute(createTableCmd);
            }
            else if (dropTableCmd != null)
            {
                return db.Execute(dropTableCmd);
            }
            else
                throw new Exception("wrong query");
        }
    }
}
