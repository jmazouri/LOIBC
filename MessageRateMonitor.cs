using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;
using LOIBC.SpamTriggers;
using Serilog;

namespace LOIBC
{
    public class MessageRateMonitor
    {
        private readonly SpamHeuristicGroup _heuristicGroup = new SpamHeuristicGroup
        {
            new LongMessageBodyHeuristic(),
            new MessageVolumeHeuristic(5),
            new RepeatedCharacterHeuristic(),
            new CapitalLetterHeuristic()
        };

        private readonly List<SpamTrigger> _triggers = new List<SpamTrigger>
        {
            new KickTrigger()
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
        }

        public void Analyze(Message message)
        {
            float spamValue = _heuristicGroup.CalculateSpamValue(message);

            Log.Logger.Information("Message from {user} had a spam rating of : {rating} ({heuristictype})", 
                message.User.Name, spamValue, _heuristicGroup.AggregateMethod);

            foreach (SpamHeuristic heuristic in _heuristicGroup)
            {
                Log.Logger.Information("\t {heuristic}: {rating} ({weight}wt)", 
                    heuristic.GetType().Name, heuristic.CalculateSpamValue(message, false), _heuristicGroup.Heuristics[heuristic]);
            }

            foreach (SpamTrigger trigger in _triggers)
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

                break;
            }
        }
    }
}
