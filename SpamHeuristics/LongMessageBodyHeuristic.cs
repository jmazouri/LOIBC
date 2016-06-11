using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public class LongMessageBodyHeuristic : SpamHeuristic
    {
        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            return sentMessage.Text.Length * 0.05f;
        }
    }
}
