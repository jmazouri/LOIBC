using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace LOIBC.SpamHeuristics
{
    public class SpamHeuristicGroup : SpamHeuristic, IEnumerable<SpamHeuristic>
    {
        public Dictionary<SpamHeuristic, float> Heuristics;

        public SpamHeuristicGroup()
        {
            Heuristics = new Dictionary<SpamHeuristic, float>();
        }

        public SpamHeuristicGroup(params SpamHeuristic[] heuristics)
        {
            Heuristics = heuristics.ToDictionary(d=>d, d=> 1f);
        }

        public void Add(SpamHeuristic heuristic, float weight = 1)
        {
            Heuristics.Add(heuristic, weight);
        }

        public override float CalculateSpamValue(Message sentMessage, bool keepCached = true)
        {
            return Heuristics.Sum(d => d.Key.CalculateSpamValue(sentMessage)*d.Value);
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
