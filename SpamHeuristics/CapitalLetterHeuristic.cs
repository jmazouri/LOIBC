using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public class CapitalLetterHeuristic : SpamHeuristic
    {
        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            return (float)sentMessage.Text.Count(d=>Char.ToUpperInvariant(d) == d) / sentMessage.Text.Length;
        }
    }
}
