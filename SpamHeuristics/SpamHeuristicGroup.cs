﻿using System;
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

    public class SpamHeuristicGroup : SpamHeuristic, IEnumerable<SpamHeuristic>
    {
        public Dictionary<SpamHeuristic, float> Heuristics;
        private HeuristicAggregateMethod _aggregateMethod;

        public SpamHeuristicGroup(HeuristicAggregateMethod aggregateMethod = HeuristicAggregateMethod.Sum)
        {
            Heuristics = new Dictionary<SpamHeuristic, float>();
            _aggregateMethod = aggregateMethod;
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
            switch (_aggregateMethod)
            {
                case HeuristicAggregateMethod.Sum:
                    return Heuristics.Sum(d => d.Key.CalculateSpamValue(sentMessage) * d.Value);
                case HeuristicAggregateMethod.Average:
                    return Heuristics.Average(d => d.Key.CalculateSpamValue(sentMessage) * d.Value);
                case HeuristicAggregateMethod.Max:
                    return Heuristics.Max(d => d.Key.CalculateSpamValue(sentMessage) * d.Value);
                default:
                    return Heuristics.Sum(d => d.Key.CalculateSpamValue(sentMessage) * d.Value);
            }
        }

        public IEnumerator<SpamHeuristic> GetEnumerator()
        {
            return Heuristics.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}