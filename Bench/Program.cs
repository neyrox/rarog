using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Bench
{
    class Program
    {
        private readonly Options options;
        
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
                if (options.Init)
                    Init();
                else
                    Benchmark();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Init()
        {
            var init = new InitJob(options, "Init");
            init.Run();
        }

        private void Benchmark()
        {
            var sw = new Stopwatch();
            sw.Start();

            var jobs = new List<Thread>();
            for (int i = 0; i < options.Jobs; ++i)
            {
                var bench = new BenchJob(options, $"Bench{i}");
                var job = new Thread(bench.Run);
                job.Start();
                jobs.Add(job);
            }

            foreach (var job in jobs)
                job.Join();

            var elapsedSeconds = sw.ElapsedMilliseconds * 0.001;
            var tps = BenchJob.TransactionsCount / elapsedSeconds; 
            Console.WriteLine($"Total: {tps} transactions per second");
        }
    }
}
