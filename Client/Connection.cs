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
            _stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
            _stream.Write(data, 0, data.Length);

            // Receive the Server response.
            data = new byte[sizeof(int)];
            var bytes = _stream.Read(data, 0, data.Length);
            var bodyLength = BitConverter.ToInt32(data, 0);

            // Buffer to store the response bytes.
            var body = new byte[bodyLength];

            bytes = _stream.Read(body, 0, body.Length);
            Console.WriteLine("Received: {0} bytes", bytes);

            //var resultBytes = new byte[bytes];
            // TODO: get rid of copying here
            //Array.Copy(data, resultBytes, bytes);
            var packer = new Engine.Serialization.MPackResultPacker();
            var result = packer.UnpackResult(body);
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