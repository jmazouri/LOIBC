using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC
{
    public class MessageRateMonitor
    {
        private FixedSizedQueue<Message> _messages;

        public MessageRateMonitor()
        {
            _messages = new FixedSizedQueue<Message>(100);
        }

        public void Add(Message message)
        {
            _messages.Enqueue(message);
        }
    }
}
