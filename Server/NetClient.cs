using System;
using System.Net.Sockets;
using Engine;

namespace Server
{
    public class NetClient
    {
        private readonly TcpClient _client;
        private readonly Shell _shell;

        public NetClient(TcpClient client, Shell shell)
        {
            _client = client;
            _shell = shell;
        }

        public void Serve()
        {
            BeginRead();
        }

        private void BeginRead()
        {
            // TODO: implement framing
            var buffer = new byte[65536];
            var stream = _client.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, EndRead, buffer);
        }

        private void EndRead(IAsyncResult asyncResult)
        {
            var buffer = (byte[])asyncResult.AsyncState;
            var stream = _client.GetStream();
            var bytesAvailable = stream.EndRead(asyncResult);

            var query = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesAvailable);
            Console.WriteLine("Received: {0}", query);

            // Process the data sent by the client.
            var result = _shell.Execute(query);
            var packer = new Engine.Serialization.MPackResultPacker();

            byte[] resp = packer.PackResult(result);

            // Send back a response.
            stream.BeginWrite(resp, 0, resp.Length, EndSend, resp);
        }
        
        private void EndSend(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            Console.WriteLine("Sent {0} bytes to client", buffer.Length);
            BeginRead();
        }
    }
}