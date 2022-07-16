using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using Engine;
using Microsoft.Extensions.Configuration;
using Rarog;

namespace Bench
{
    class Program
    {
        private Connection dbConn;
        private Options options;
        private Random rnd;

        public class Options
        {
            public static readonly Dictionary<string, string> CommandLineMap = new Dictionary<string, string>
            {
                {"-i", "init"},
            };

            public bool Init { get; set; }
        }
        
        static void Main(string[] args)
        {
            var program = new Program(args);
            program.Run();
        }

        private Program(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args, Options.CommandLineMap)
                .Build();

            options = new Options();
            config.Bind(options);
        }

        public void Run()
        {
            try
            {
                int port = 33777;
                dbConn = new Connection("localhost", port);
                Console.WriteLine("Connected");

                rnd = new Random(DateTime.UtcNow.ToFileTimeUtc().GetHashCode());

                try
                {
                    if (options.Init)
                        Init();
                    else
                        Benchmark();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    // Close everything.
                    dbConn.Close();
                }
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
        
        private void Init()
        {
            Console.WriteLine("Dropping tables");
            var dropTable1 = Perform("DROP TABLE bench_accounts;");
            if (!dropTable1.IsOK)
                throw new Exception("Failed to drop database bench_accounts");
            var dropTable2 = Perform("DROP TABLE bench_branches;");
            if (!dropTable2.IsOK)
                throw new Exception("Failed to drop database bench_branches");
            var dropTable3 = Perform("DROP TABLE bench_history;");
            if (!dropTable3.IsOK)
                throw new Exception("Failed to drop database bench_history");
            var dropTable4 = Perform("DROP TABLE bench_tellers;");
            if (!dropTable4.IsOK)
                throw new Exception("Failed to drop database bench_tellers");

            var createTable1 = Perform("CREATE TABLE bench_accounts (aid int, bid int, abalance int, filler varchar(96));");
            if (!createTable1.IsOK)
                throw new Exception("Failed to initialize database bench_accounts");

            var createTable2 = Perform("CREATE TABLE bench_branches (bid int, bbalance int, filler varchar(88));");
            if (!createTable2.IsOK)
                throw new Exception("Failed to initialize database bench_branches");

            var createTable3 = Perform("CREATE TABLE bench_history (tid int, bid int, aid int, delta int, mtime bigint, filler varchar(88));");
            if (!createTable3.IsOK)
                throw new Exception("Failed to initialize database bench_history");

            var createTable4 = Perform("CREATE TABLE bench_tellers (tid int, bid int, tbalance int, filler varchar(88));");
            if (!createTable4.IsOK)
                throw new Exception("Failed to initialize database bench_tellers");

            var fillBuff = new byte[32];
            var sw = new Stopwatch();
            sw.Start();
            for (int aid = 0; aid < 10000; ++aid)
            {
                int bid = rnd.Next(1);
                //int tid = rnd.Next(10);
                rnd.NextBytes(fillBuff);
                var filler = BitConverter.ToString(fillBuff);
                var insert = Perform($"INSERT INTO bench_accounts (aid, bid, abalance, filler) VALUES ({aid}, {bid}, 0, {filler});");
                if (!insert.IsOK)
                    throw new Exception("Failed to insert account");

                if (aid % 1000 == 999)
                {
                    var elapsedSeconds = sw.ElapsedMilliseconds * 0.001;
                    sw.Restart();
                    var qps = 1000 / elapsedSeconds;
                    Console.WriteLine($"Insert: {qps} Queries per second");
                }
            }

            for (int bid = 0; bid < 1; ++bid)
            {
                //int bid = rnd.Next(1);
                //int tid = rnd.Next(10);
                rnd.NextBytes(fillBuff);
                var filler = BitConverter.ToString(fillBuff);
                var insert = Perform($"INSERT INTO bench_branches (bid, bbalance, filler) VALUES ({bid}, 0, {filler});");
                if (!insert.IsOK)
                    throw new Exception("Failed to insert branch");
            }

            for (int tid = 0; tid < 10; ++tid)
            {
                int bid = rnd.Next(1);
                rnd.NextBytes(fillBuff);
                var filler = BitConverter.ToString(fillBuff);
                var insert = Perform($"INSERT INTO bench_tellers (tid, bid, tbalance, filler) VALUES ({tid}, {bid}, 0, {filler});");
                if (!insert.IsOK)
                    throw new Exception("Failed to insert teller");
            }
        }

        private void Benchmark()
        {
            int aid = rnd.Next(10000);
            int delta = rnd.Next(10000) - 5000;

            var update = Perform($"UPDATE bench_accounts SET abalance = abalance + {delta} WHERE aid = {aid};");
        }

        private void RunTransaction()
        {
            //var result = dbConn.Perform("CREATE TABLE bench_accounts (aid int, bid int, abalance int, filler varchar(255);");
        }

        private Result Perform(string query)
        {
            var result = dbConn.Perform(query);
            if (!result.IsOK)
                Console.WriteLine(result.Error);
            return result;
        }
    }
}
