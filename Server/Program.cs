﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Engine;
using Engine.Storage;

namespace Server
{
    public interface Callbacks
    {
        void OnDisconnect(long clientId);
    }

    class Program : Callbacks
    {
        private static readonly Log Log = LogManager.Create<Program>();

        // Thread signal.
        private readonly ManualResetEvent _tcpClientConnected = new ManualResetEvent(false);
        private readonly FileStorage _fileStorage = new FileStorage(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
        private readonly Database _db;
        private readonly Shell _shell;
        private readonly ConcurrentDictionary<long, NetClient> _clients = new ConcurrentDictionary<long, NetClient>();
        private readonly ManualResetEvent _stopping = new ManualResetEvent(false);
        private Thread _flushThread;
        private long _nextClientId = 0;

        private Program()
        {
            _db = new Database(_fileStorage);
            _db.Load();
            _shell = new Shell(_db);
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.Start();
        }

        void Start()
        {
            TcpListener server = null;
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
            catch (SocketException e)
            {
                Log.Error(e);
                server?.Stop();
                server = null;
            }

            if (server != null)
            {
                Log.Info("Server Started");
            }
            else
            {
                Log.Error("Server Failed to Start");
                return;
            }

            _flushThread = new Thread(Flush) {IsBackground = true};
            _flushThread.Start();

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
                Log.Error(e);
            }
            finally
            {
                _stopping.Set();

                // Stop listening for new clients.
                server.Stop();

                _flushThread.Join();
            }
        }

        // Accept one client connection asynchronously.
        private void BeginAcceptClient(TcpListener listener)
        {
            // Set the event to nonsignaled state.
            _tcpClientConnected.Reset();

            // Start to listen for connections from a client.
            Log.Info("Waiting for a connection...");

            // Accept the connection.
            // BeginAcceptSocket() creates the accepted socket.
            listener.BeginAcceptTcpClient(EndAccept, listener);

            // Wait until a connection is made and processed before
            // continuing.
            _tcpClientConnected.WaitOne();
        }

        // Process the client connection.
        private void EndAccept(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            var listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on
            // the console.
            var client = listener.EndAcceptTcpClient(ar);

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)
            var clientId = _nextClientId++;
            Log.Debug($"Client {clientId} connected");
            NetClient netClient = new NetClient(clientId, client, new Shell(_db), this);
            if (_clients.TryAdd(_nextClientId++, netClient))
            {
                netClient.Serve();
            }

            // Signal the calling thread to continue.
            _tcpClientConnected.Set();
        }

        private void Flush()
        {
            while (!_stopping.WaitOne(0))
            {
                _shell.Execute("FLUSH;");

                if (_stopping.WaitOne(10000))
                    break;
            }
        }

        public void OnDisconnect(long clientId)
        {
            Log.Debug($"Client {clientId} disconnected");
            _clients.TryRemove(clientId, out var client);
        }
    }
}
