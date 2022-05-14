using System;
using System.Net.Sockets;
using Engine;
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

                    var result = dbConn.Perform(query);
                    Print(result);
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

        static void Print(Result result)
        {
            if (result.IsOK)
            {
                if (result.Columns == null || result.Columns.Count == 0)
                {
                    Console.WriteLine("OK");
                    return;
                }

                int colCount = result.Columns.Count;
                var maxWidths = new int[colCount];
                for (int i = 0; i < colCount; ++i)
                {
                    var column = result.Columns[i];
                    var maxWidth = column.Name.Length;
                    for (int j = 0; j < column.Count; ++j)
                    {
                        var cellLength = column.Get(j).Length;
                        if (cellLength > maxWidth)
                            maxWidth = cellLength;
                    }
                    maxWidths[i] = maxWidth;
                }

                for (int i = 0; i < colCount; ++i)
                {
                    var column = result.Columns[i];
                    Console.Write(column.Name.PadRight(maxWidths[i]));
                    if (i < (colCount - 1))
                        Console.Write(" | ");
                }
                Console.WriteLine("");
                for (int i = 0; i < colCount; ++i)
                {
                    for (int j = 0; j < maxWidths[i]; ++j)
                        Console.Write("-");
                    if (i < (colCount - 1))
                        Console.Write("-+-");
                }
                Console.WriteLine("");

                int rowCount = result.Columns[0].Count;
                for (int i = 0; i < rowCount; ++i)
                {
                    for (int j = 0; j < colCount; ++j)
                    {
                        var column = result.Columns[j];
                        Console.Write(column.Get(i).PadRight(maxWidths[j]));
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
