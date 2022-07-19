using System.Collections.Generic;

namespace Bench
{
    public class Options
    {
        public static readonly Dictionary<string, string> CommandLineMap = new Dictionary<string, string>
        {
            {"-i", "init"},
            {"-j", "jobs"},
        };

        public bool Init { get; set; }
        public int Jobs { get; set; } = 1;
    }
}
