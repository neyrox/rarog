using System;
using System.Net.Sockets;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 33777;
                TcpClient client = new TcpClient("localhost", port);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();
                NetworkStream stream = client.GetStream();

                Console.WriteLine("Connected");

                while (true)
                {
                    var query = Console.ReadLine();
                    var lq = query.ToLower();
                    if (lq == "quit" || lq == "exit")
                        break;

                    Perform(stream, query);
                }

                // Close everything.
                stream.Close();
                client.Close();
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

        static void Perform(NetworkStream stream, string query)
        {
            // Translate the passed message into UTF8 and store it as a Byte array.
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(query);

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", query);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[65536];

            // String to store the response UTF8 representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            Console.WriteLine("Received: {0} bytes", bytes);
            
            var resultBytes = new byte[bytes];
            // TODO: get rid of copying here
            Array.Copy(data, resultBytes, bytes);
            var packer = new Engine.Serialization.MPackResultPacker();
            var result = packer.UnpackResult(resultBytes);
            if (result.IsOK)
            {
                if (result.Columns == null || result.Columns.Count == 0)
                    return;

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