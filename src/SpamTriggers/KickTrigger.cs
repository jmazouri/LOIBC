using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamTriggers
{
    public class KickTrigger : SpamTrigger
    {
        public override void Trigger(DiscordClient client, Message message)
        {
            message.User.Kick().Wait();
        }
    }
}
