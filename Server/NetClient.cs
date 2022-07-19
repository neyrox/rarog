using System;
using System.Net.Sockets;
using Engine;

namespace Server
{
    public class NetClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly Shell _shell;
        private readonly byte[] _requestLength = new byte[4];

        public NetClient(TcpClient client, Shell shell)
        {
            _client = client;
            _shell = shell;

            _stream = _client.GetStream();
        }

        public void Serve()
        {
            BeginRead();
        }

        private void BeginRead()
        {
            // TODO: implement framing
            _stream.BeginRead(_requestLength, 0, _requestLength.Length, EndReadHeader, null);
        }

        private void EndReadHeader(IAsyncResult asyncResult)
        {
            var bytesAvailable = _stream.EndRead(asyncResult);
            if (bytesAvailable != 4)
            {
                //handle loading more
                return;
            }

            int length = BitConverter.ToInt32(_requestLength);

            // TODO: use pools
            var buffer = new byte[length];
            _stream.BeginRead(buffer, 0, buffer.Length, EndReadBody, buffer);
        }

        private void EndReadBody(IAsyncResult asyncResult)
        {
            var buffer = (byte[])asyncResult.AsyncState;
            var bytesAvailable = _stream.EndRead(asyncResult);

            var query = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesAvailable);
            Console.WriteLine("Received: {0}", query);

            // Process the data sent by the client.
            var result = _shell.Execute(query);
            var packer = new Engine.Serialization.MPackResultPacker();

            byte[] resp = packer.PackResult(result);

            // Send back a response.
            var head = BitConverter.GetBytes(resp.Length);
            _stream.BeginWrite(head, 0, head.Length, EndSendHeader, resp);
        }

        private void EndSendHeader(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            _stream.BeginWrite(buffer, 0, buffer.Length, EndSendBody, buffer);
        }

        private void EndSendBody(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            //Console.WriteLine("Sent {0} bytes to client", buffer.Length);
            BeginRead();
        }
    }
}