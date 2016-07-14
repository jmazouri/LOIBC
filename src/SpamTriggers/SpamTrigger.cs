using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace LOIBC.SpamTriggers
{
    public abstract class SpamTrigger
    {
        public float TriggerScore { get; set; }
        public bool PassThrough { get; set; } = true;

        public virtual bool ShouldTrigger(float spamScore) => spamScore >= TriggerScore;
        public virtual string Name => GetType().Name;
        public abstract void Trigger(DiscordClient client, Message message);
    }
}
