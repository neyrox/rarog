using System;
using System.Net;
using System.Net.Sockets;
using Engine;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database();
            var shell = new Shell(db);

            TcpListener server=null;
            try
            {
                // Set the TcpListener on port 33777.
                Int32 port = 33777;
                IPAddress localAddr = IPAddress.Any;

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server?.Stop();
                server = null;
            }

            if (server != null)
            {
                Console.WriteLine("Server Started");
            }
            else
            {
                Console.WriteLine("Server Failed to Start");
                return;
            }
            
            try
            {
                // Enter the listening loop.
                while(true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    HandleClient(client, shell);
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

        }

        private static void HandleClient(TcpClient client, Shell shell)
        {
            Console.WriteLine("Connected!");

            // Buffer for reading data
            Byte[] bytes = new Byte[65536];
            String query = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                query = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", query);

                // Process the data sent by the client.
                var result = shell.Execute(query);
                var packer = new Engine.Serialization.MPackResultPacker();

                byte[] msg = packer.PackResult(result);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0} bytes", msg.Length);
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}