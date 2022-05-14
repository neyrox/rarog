﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Engine;

namespace Server
{
    class Program
    {
        // Thread signal.
        private static readonly ManualResetEvent _tcpClientConnected = new ManualResetEvent(false);
        private static readonly Database _db = new Database();
        private static readonly Shell _shell = new Shell(_db);
        // TODO: implement removal on disconnect
        private static readonly ConcurrentBag<NetClient> _clients = new ConcurrentBag<NetClient>();

        static void Main(string[] args)
        {
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
                    BeginAcceptClient(server);
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

        // Accept one client connection asynchronously.
        private static void BeginAcceptClient(TcpListener listener)
        {
            // Set the event to nonsignaled state.
            _tcpClientConnected.Reset();

            // Start to listen for connections from a client.
            Console.WriteLine("Waiting for a connection...");

            // Accept the connection.
            // BeginAcceptSocket() creates the accepted socket.
            listener.BeginAcceptTcpClient(EndAccept, listener);

            // Wait until a connection is made and processed before
            // continuing.
            _tcpClientConnected.WaitOne();
        }

        // Process the client connection.
        private static void EndAccept(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            var listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on
            // the console.
            var client = listener.EndAcceptTcpClient(ar);

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)
            Console.WriteLine("Client connected");
            NetClient netClient = new NetClient(client, _shell);
            _clients.Add(netClient);
            netClient.Serve();

            // Signal the calling thread to continue.
            _tcpClientConnected.Set();
        }
    }
}