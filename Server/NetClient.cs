using System;
using System.Net.Sockets;
using Common;
using Engine;

namespace Server
{
    public class NetClient
    {
        private static readonly Log Log = LogManager.Create<NetClient>();

        private readonly long _id;
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly Shell _shell;
        private readonly Callbacks _cbs;
        private readonly byte[] _requestLength = new byte[4];
        private int _offset = 0;

        public NetClient(long id, TcpClient client, Shell shell, Callbacks cbs)
        {
            _id = id;
            _client = client;
            _shell = shell;
            _cbs = cbs;

            _stream = _client.GetStream();
        }

        public void Serve()
        {
            BeginRead();
        }

        private void BeginRead()
        {
            _offset = 0;
            _stream.BeginRead(_requestLength, _offset, _requestLength.Length, EndReadHeader, null);
        }

        private void EndReadHeader(IAsyncResult asyncResult)
        {
            var bytesAvailable = _stream.EndRead(asyncResult);
            if (bytesAvailable != 4)
            {
                if (bytesAvailable == 0)
                {
                    Log.Debug("Read 0 bytes");
                    _cbs.OnDisconnect(_id);
                }
                else
                {
                    Log.Debug("Read more");
                    _offset += bytesAvailable;
                    _stream.BeginRead(_requestLength, _offset, _requestLength.Length - _offset, EndReadHeader, null);
                }
                return;
            }

            int length = BitConverter.ToInt32(_requestLength);

            // TODO: use pools
            _offset = 0;
            var buffer = new byte[length];
            _stream.BeginRead(buffer, _offset, buffer.Length, EndReadBody, buffer);
        }

        private void EndReadBody(IAsyncResult asyncResult)
        {
            var buffer = (byte[])asyncResult.AsyncState;
            var bytesAvailable = _stream.EndRead(asyncResult);
            if (bytesAvailable != buffer.Length)
            {
                if (bytesAvailable == 0)
                {
                    Log.Debug("Read 0 bytes");
                    _cbs.OnDisconnect(_id);
                }
                else
                {
                    Log.Debug("Read more");
                    _offset += bytesAvailable;
                    _stream.BeginRead(buffer, _offset, buffer.Length - _offset, EndReadBody, buffer);
                }
                return;
            }

            var query = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesAvailable);
            //Console.WriteLine("Received: {0}", query);

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