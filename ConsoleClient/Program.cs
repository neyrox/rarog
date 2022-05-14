using System;
using System.Net.Sockets;
using Rarog;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = 33777;
                var dbConn = new Connection("localhost", port);
                Console.WriteLine("Connected");

                while (true)
                {
                    var query = Console.ReadLine();
                    var lq = query.ToLower();
                    if (lq == "quit" || lq == "exit")
                        break;

                    Perform(dbConn, query);
                }

                // Close everything.
                dbConn.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void Perform(Connection dbConn, string query)
        {
            var result = dbConn.Perform(query);
            if (result.IsOK)
            {
                if (result.Columns == null || result.Columns.Count == 0)
                {
                    Console.WriteLine("OK");
                    return;
                }

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
            else
            {
                Console.WriteLine("Error: " + result.Error);
            }
        }
    }
}