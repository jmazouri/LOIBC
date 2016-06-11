using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamTriggers
{
    public abstract class SpamTrigger
    {
        public float TriggerScore { get; set; }

        public virtual bool ShouldTrigger(float spamScore) => spamScore >= TriggerScore;
        public abstract void Trigger(DiscordClient client, Message message);
    }
}
