using System;
using System.Diagnostics;

namespace Bench
{
    public class InitJob : JobBase
    {
        public InitJob(Options options, string name)
            : base(options, name)
        {
        }

        protected override void RunInternal()
        {
            Console.WriteLine("Dropping tables");
            var dropTable1 = Perform("DROP TABLE bench_accounts IF EXISTS;");
            if (!dropTable1.IsOK)
                throw new Exception("Failed to drop database bench_accounts");
            var dropTable2 = Perform("DROP TABLE bench_branches IF EXISTS;");
            if (!dropTable2.IsOK)
                throw new Exception("Failed to drop database bench_branches");
            var dropTable3 = Perform("DROP TABLE bench_history IF EXISTS;");
            if (!dropTable3.IsOK)
                throw new Exception("Failed to drop database bench_history");
            var dropTable4 = Perform("DROP TABLE bench_tellers IF EXISTS;");
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
            var scale = options.Scale;
            for (int aid = 0; aid < 10000 * scale; ++aid)
            {
                int bid = rnd.Next(scale);
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

            for (int bid = 0; bid < scale; ++bid)
            {
                rnd.NextBytes(fillBuff);
                var filler = BitConverter.ToString(fillBuff);
                var insert = Perform($"INSERT INTO bench_branches (bid, bbalance, filler) VALUES ({bid}, 0, {filler});");
                if (!insert.IsOK)
                    throw new Exception("Failed to insert branch");
            }

            for (int tid = 0; tid < 10 * scale; ++tid)
            {
                int bid = rnd.Next(scale);
                rnd.NextBytes(fillBuff);
                var filler = BitConverter.ToString(fillBuff);
                var insert = Perform($"INSERT INTO bench_tellers (tid, bid, tbalance, filler) VALUES ({tid}, {bid}, 0, {filler});");
                if (!insert.IsOK)
                    throw new Exception("Failed to insert teller");
            }
        }
    }
}
