using System;
using System.Diagnostics;
using System.Threading;

namespace Bench
{
    public class BenchJob : JobBase
    {
        public static volatile int TransactionsCount;
        
        public BenchJob(Options options, string name)
            : base(options, name)
        {
        }

        protected override void RunInternal()
        {
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 100; j++)
                    RunTransaction();

                var elapsedSeconds = sw.ElapsedMilliseconds * 0.001;
                sw.Restart();
                var tps = 100 / elapsedSeconds;
                Console.WriteLine($"{name}: {tps} transactions per second");
            }
        }

        private void RunTransaction()
        {
            var scale = options.Scale;

            int aid = rnd.Next(10000 * scale);
            int bid = rnd.Next(scale);
            int tid = rnd.Next(10 * scale);
            int delta = rnd.Next(10000) - 5000;
            int deltaAbs = Math.Abs(delta);
            char sign = delta < 0 ? '-' : '+';
            var mtime = DateTime.UtcNow.ToFileTimeUtc();

            Perform("BEGIN;");
            var update1 = Perform($"UPDATE bench_accounts SET abalance = abalance {sign} {deltaAbs} WHERE aid = {aid};");
            var select = Perform($"SELECT abalance FROM bench_accounts WHERE aid = {aid};");
            var update2 = Perform($"UPDATE bench_tellers SET tbalance = tbalance {sign} {deltaAbs} WHERE tid = {tid};");
            var update3 = Perform($"UPDATE bench_branches SET bbalance = bbalance {sign} {deltaAbs} WHERE bid = {bid};");
            var insert = Perform($"INSERT INTO bench_history (tid, bid, aid, delta, mtime) VALUES ({tid}, {bid}, {aid}, {delta}, {mtime});");
            Perform("END;");

            Interlocked.Increment(ref TransactionsCount);
        }
    }
}