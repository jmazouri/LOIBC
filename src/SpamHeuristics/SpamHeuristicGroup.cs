using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public enum HeuristicAggregateMethod
    {
        Sum,
        Average,
        Max
    }

    public class SpamHeuristicGroup : SpamHeuristic
    {
        public Dictionary<SpamHeuristic, float> Heuristics { get; set; }
        public HeuristicAggregateMethod AggregateMethod { get; set; }

        public SpamHeuristicGroup(HeuristicAggregateMethod aggregateMethod = HeuristicAggregateMethod.Sum)
        {
            Heuristics = new Dictionary<SpamHeuristic, float>();
            AggregateMethod = aggregateMethod;
        }

        public SpamHeuristicGroup(HeuristicAggregateMethod aggregateMethod, params SpamHeuristic[] heuristics)
        {
            Heuristics = heuristics.ToDictionary(d=>d, d=> 1f);
        }

        public void Add(SpamHeuristic heuristic, float weight = 1)
        {
            Heuristics.Add(heuristic, weight);
        }

        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            switch (AggregateMethod)
            {
                case HeuristicAggregateMethod.Sum:
                    return Heuristics.Sum(d => d.Key.CalculateSpamValue(sentMessage, keepCached) * d.Value);
                case HeuristicAggregateMethod.Average:
                    return Heuristics.Average(d => d.Key.CalculateSpamValue(sentMessage, keepCached) * d.Value);
                case HeuristicAggregateMethod.Max:
                    return Heuristics.Max(d => d.Key.CalculateSpamValue(sentMessage, keepCached) * d.Value);
                default:
                    return Heuristics.Sum(d => d.Key.CalculateSpamValue(sentMessage, keepCached) * d.Value);
            }
        }
    }
}
