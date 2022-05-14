using System;
using System.Net.Sockets;
using Engine;

namespace Rarog
{
    public class Connection
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public Connection(string host, int port)
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            _tcpClient = new TcpClient(host, port);

            // Get a client stream for reading and writing.
            _stream = _tcpClient.GetStream();
        }

        public Result Perform(string query)
        {
            // Translate the passed message into UTF8 and store it as a Byte array.
            var data = System.Text.Encoding.UTF8.GetBytes(query);

            // Send the message to the connected TcpServer.
            _stream.Write(data, 0, data.Length);
            
            Console.WriteLine("Sent: {0}", query);

            // Receive the Server response.

            // Buffer to store the response bytes.
            data = new Byte[65536];

            // Read the first batch of the TcpServer response bytes.
            var bytes = _stream.Read(data, 0, data.Length);
            Console.WriteLine("Received: {0} bytes", bytes);
            
            var resultBytes = new byte[bytes];
            // TODO: get rid of copying here
            Array.Copy(data, resultBytes, bytes);
            var packer = new Engine.Serialization.MPackResultPacker();
            var result = packer.UnpackResult(resultBytes);
            return result;
        }

        public void Close()
        {
            // Close everything.
            _stream.Close();
            _tcpClient.Close();
        }
    }
}