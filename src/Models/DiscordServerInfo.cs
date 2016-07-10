using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace LOIBC
{
    public class DiscordServerInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }
        public string Icon { get; set; }

        [Ignore]
        public Dictionary<string, bool> Channels { get; set; }
    }
}
