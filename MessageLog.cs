using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LOIBC.WebInterface.ViewModels;

namespace LOIBC
{
    public class MessageLog
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
        public DiscordServerInfo Server { get; set; }
        public string Channel { get; set; }
        public string Trigger { get; set; }
        public string Formula { get; set; }

        public float SpamValue { get; set; }

        public List<string> Details { get; set; } = new List<string>();
    }
}
