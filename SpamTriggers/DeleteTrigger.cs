using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamTriggers
{
    public class DeleteTrigger : SpamTrigger
    {
        public override void Trigger(DiscordClient client, Message message)
        {
            message.User.SendMessage("Your message was deleted because you triggered a spam filter. Please calm down.").Wait();
            message.Delete().Wait();
        }
    }
}
