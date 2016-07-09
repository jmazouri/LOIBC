using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public class RepeatedCharacterHeuristic : SpamHeuristic
    {
        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            return sentMessage.Text.GroupBy(d => d).Max(d => d.Count()) * 0.5f;
        }
    }
}
