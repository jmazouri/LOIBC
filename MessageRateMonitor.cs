using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;

namespace LOIBC
{
    public class MessageRateMonitor
    {
        private readonly SpamHeuristicGroup _heuristicGroup = new SpamHeuristicGroup
        {
            new LongMessageBodyHeuristic(),
            new MessageRateHeuristic(5),
            new RepeatedCharacterHeuristic()
        };

        public void Analyze(Message message)
        {
            Console.WriteLine($"Last message had a spam rating of : {_heuristicGroup.CalculateSpamValue(message)}");

            foreach (SpamHeuristic heuristic in _heuristicGroup)
            {
                Console.WriteLine($"\t {heuristic.GetType().Name}: {heuristic.CalculateSpamValue(message, false)} " +
                                  $"(Weight: {_heuristicGroup.Heuristics[heuristic]})");
            }
        }
    }
}
