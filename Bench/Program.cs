using System;
using System.Net.Sockets;
using Engine;
using Rarog;

namespace Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = 33777;
                var dbConn = new Connection("localhost", port);
                Console.WriteLine("Connected");

                Init(dbConn);

                Benchmark(dbConn);
                
                // Close everything.
                dbConn.Close();
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

        static void Init(Connection dbConn)
        {
            var testTable1 = dbConn.Perform("SELECT * FROM TABLE bench_accounts LIMIT 1;");
            if (testTable1.IsOK)
                return;

            var createTable1 = dbConn.Perform("CREATE TABLE bench_accounts (aid int, bid int, abalance int, filler varchar(84);");
            if (!createTable1.IsOK)
                throw new Exception("Failed to initialize database");

            var createTable2 = dbConn.Perform("CREATE TABLE bench_branches (bid int, bbalance int, filler varchar(88);");
            if (!createTable2.IsOK)
                throw new Exception("Failed to initialize database");

            var createTable3 = dbConn.Perform("CREATE TABLE bench_history (tid int, bid int, aid int, delta int, mtime long, filler varchar(88);");
            if (!createTable3.IsOK)
                throw new Exception("Failed to initialize database");

            var createTable4 = dbConn.Perform("CREATE TABLE bench_tellers (tid int, bid int, tbalance int, filler varchar(88);");
            if (!createTable4.IsOK)
                throw new Exception("Failed to initialize database");
        }

        static void Benchmark(Connection dbConn)
        {
            //var result = dbConn.Perform("CREATE TABLE bench_accounts (aid int, bid int, abalance int, filler varchar(255);");
        }

        static void RunTransaction(Connection dbConn)
        {
            //var result = dbConn.Perform("CREATE TABLE bench_accounts (aid int, bid int, abalance int, filler varchar(255);");
        }
    }
}
