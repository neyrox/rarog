using Engine;
using System;

namespace Rarog
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
                    if (result.Columns == null || result.Columns.Count == 0)
                        continue;

                    int rowCount = result.Columns[0].Count;
                    int colCount = result.Columns.Count;

                    for (int i = 0; i < rowCount; ++i)
                    {
                        for (int j = 0; j < colCount; ++j)
                        {
                            var column = result.Columns[j];
                            Console.Write(column.Get(i));
                            if (j < (colCount - 1))
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
