using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public class MessageRateHeuristic : SpamHeuristic
    {
        private FixedSizedQueue<Message> _messages;

        public MessageRateHeuristic(int maxMessageHistory = 25)
        {
            _messages = new FixedSizedQueue<Message>(maxMessageHistory);
        }
        
        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            if (keepCached)
            {
                _messages.Enqueue(sentMessage);
            }

            return _messages.AsEnumerable().Count(d => d.User == sentMessage.User) * 0.25f + 
                   _messages.AsEnumerable().Count(d => d.Text == sentMessage.Text);
        }
    }
}
