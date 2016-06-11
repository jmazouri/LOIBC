using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public abstract class SpamHeuristic
    {
        public abstract float CalculateSpamValue(Message sentMessage, bool keepCached = true);
    }
}
