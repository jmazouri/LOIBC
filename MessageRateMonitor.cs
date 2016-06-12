using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;
using Serilog;

namespace LOIBC
{
    public class MessageRateMonitor
    {
        private readonly SpamHeuristicGroup _heuristicGroup = new SpamHeuristicGroup
        {
            new LongMessageBodyHeuristic(),
            new MessageRateHeuristic(5),
            new RepeatedCharacterHeuristic(),
            new CapitalLetterHeuristic()
        };

        public void Analyze(Message message)
        {
            Log.Logger.Information("Message from {user} had a spam rating of : {rating} ({heuristictype})", 
                message.User.Name, _heuristicGroup.CalculateSpamValue(message), _heuristicGroup.AggregateMethod);

            foreach (SpamHeuristic heuristic in _heuristicGroup)
            {
                Log.Logger.Information("\t {heuristic}: {rating} ({weight}wt)", 
                    heuristic.GetType().Name, heuristic.CalculateSpamValue(message, false), _heuristicGroup.Heuristics[heuristic]);
            }
        }
    }
}
