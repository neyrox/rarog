using System.Collections.Generic;

namespace Bench
{
    public class Options
    {
        public static readonly Dictionary<string, string> CommandLineMap = new Dictionary<string, string>
        {
            {"-i", "init"},
            {"-j", "jobs"},
            {"-s", "scale"},
        };

        public bool Init { get; set; }
        public int Scale { get; set; } = 1;
        public int Jobs { get; set; } = 1;
    }
}
