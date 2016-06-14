using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;
using LOIBC.SpamTriggers;
using Newtonsoft.Json;
using Serilog;

namespace LOIBC
{
    public class MessageRateMonitor
    {
        public SpamHeuristicGroup Heuristics { get; set; } = new SpamHeuristicGroup
        {
            Heuristics = new Dictionary<SpamHeuristic, float>
            {
                { new LongMessageBodyHeuristic(), 1},
                { new MessageVolumeHeuristic(5), 1},
                { new RepeatedCharacterHeuristic(), 1},
                { new CapitalLetterHeuristic(), 1}
            }
        };

        public List<SpamTrigger> Triggers = new List<SpamTrigger>
        {
            new WarningTrigger
            {
                TriggerScore = 10
            },
            new DeleteTrigger
            {
                TriggerScore = 15
            },
            new KickTrigger
            {
                TriggerScore = 30
            }
        };

        private readonly DiscordClient _client;

        public MessageRateMonitor(DiscordClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (client.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Discord client must be connected");
            }

            _client = client;
        }

        public void Analyze(Message message)
        {
            float spamValue = Heuristics.CalculateSpamValue(message);

            Log.Logger.Information("Message from {user} had a spam rating of : {rating} ({heuristictype})", 
                message.User.Name, spamValue, Heuristics.AggregateMethod);

            foreach (SpamHeuristic heuristic in Heuristics.Heuristics.Keys)
            {
                Log.Logger.Information("\t {heuristic}: {rating} ({weight}wt)", 
                    heuristic.GetType().Name, heuristic.CalculateSpamValue(message, false), Heuristics.Heuristics[heuristic]);
            }

            foreach (SpamTrigger trigger in Triggers.OrderByDescending(d=>d.TriggerScore))
            {
                if (!trigger.ShouldTrigger(spamValue)) continue;

                Log.Logger.Information("Message from {user} caused a trigger: {trigger} ({messagescore}/{triggerscore})", 
                    message.User.Name, trigger.GetType().Name, spamValue, trigger.TriggerScore);

                try
                {
                    trigger.Trigger(_client, message);
                }
                catch (AggregateException ex)
                {
                    Log.Logger.Error(ex.InnerExceptions.FirstOrDefault(), "Tried to perform trigger {trigger}, but got an error", trigger.GetType().Name);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Tried to perform trigger {trigger}, but got an error", trigger.GetType().Name);
                }

                if (!trigger.PassThrough)
                {
                    break;
                }
            }
        }
    }
}
