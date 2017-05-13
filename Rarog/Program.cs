using Engine;
using System;

namespace NxDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database();
            var shell = new Shell(db);
            var query = string.Empty;
            while (true)
            {
                Console.Write("#: ");
                query = Console.ReadLine();
                if (query.StartsWith("exit"))
                    break;

                try
                {
                    var result = shell.Execute(query);
                    if (result.Rows == null)
                        continue;

                    for (int i = 0; i < result.Rows.Count; ++i)
                    {
                        var row = result.Rows[i];
                        for (int j = 0; j < row.Count; ++j)
                        {
                            Console.Write(row[j]);
                            if (j < (row.Count - 1))
                                Console.Write(" | ");
                        }
                        Console.WriteLine("");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing query: " + ex.ToString());
                }
            }
        }
    }
}
