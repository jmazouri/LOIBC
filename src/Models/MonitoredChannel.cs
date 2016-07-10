using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace LOIBC
{
    public class MonitoredChannel
    {
        [PrimaryKey, Unique]
        public ulong ChannelId { get; set; }
        public bool IsMonitored { get; set; }
    }
}
