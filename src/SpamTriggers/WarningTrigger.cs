using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamTriggers
{
    public class WarningTrigger : SpamTrigger
    {
        public override void Trigger(DiscordClient client, Message message)
        {
            message.User.SendMessage($"Warning: You are nearing spam limits for the server {message.Server.Name}")
                .Wait();
        }
    }
}
