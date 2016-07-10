using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;

namespace LOIBC
{
    public class MessageLog
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
        public string Channel { get; set; }
        public string Trigger { get; set; }
        public string Formula { get; set; }

        public float SpamValue { get; set; }

        [Ignore]
        public DiscordServerInfo Server { get; set; }

        [Ignore]
        public List<string> Details { get; set; } = new List<string>();

        [JsonIgnore]
        [Obsolete("Don't directly set this, serialization purposes only.")]
        public string JsonDetails
        {
            get
            {
                return JsonConvert.SerializeObject(Details);
            }
            set
            {
                Details = value == null ? null : JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        [JsonIgnore]
        [Obsolete("Don't directly set this, serialization purposes only.")]
        public string JsonServer
        {
            get
            {
                return JsonConvert.SerializeObject(Server);
            }
            set
            {
                Server = value == null ? null : JsonConvert.DeserializeObject<DiscordServerInfo>(value);
            }
        }
    }
}
