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
            float spamFactor = 0.05f;

            if (sentMessage.Text.Length < 6)
            {
                spamFactor = 0.3f;
            }

            return sentMessage.Text.Length * spamFactor;
        }
    }
}