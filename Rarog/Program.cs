using Engine;
using System;

namespace NxDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database();
            var query = string.Empty;
            while (true)
            {
                Console.Write("#: ");
                query = Console.ReadLine();
                if (query.StartsWith("exit"))
                    break;

                try
                {
                    var tokens = Lexer.Split(query);
                    var command = Parser.Convert(tokens);
                    var selectCmd = command as SelectNode;
                    var insertCmd = command as InsertNode;
                    var createTableCmd = command as CreateTableNode;
                    if (selectCmd != null)
                    {
                        var result = db.Execute(selectCmd);
                        for (int i = 0; i < result.Count; ++i)
                        {
                            var row = result[i];
                            for (int j = 0; j < row.Count; ++j)
                            {
                                Console.Write(row[j]);
                                if (j < (row.Count - 1))
                                    Console.Write(" | ");
                            }
                            Console.WriteLine("");
                        }
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing query: " + ex.ToString());
                }
            }

        }
    }
}
