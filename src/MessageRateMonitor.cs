using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.Database;
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
                { new MessageVolumeHeuristic(), 1},
                { new RepeatedCharacterHeuristic(), 1},
                { new CapitalLetterHeuristic(), 1}
            }
        };

        private IDatabaseContext _dbContext;

        private List<MessageLog> _messages = new List<MessageLog>();
        public ReadOnlyCollection<MessageLog> MessageLog => _messages.AsReadOnly();

        private Dictionary<ulong, bool> _channelsMonitored { get; set; }

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

        public MessageRateMonitor(DiscordClient client, IDatabaseContext dbContext)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (client.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Discord client must be connected");
            }

            _channelsMonitored = new Dictionary<ulong, bool>();

            _dbContext = dbContext;
            _client = client;
        }

        public async Task LoadFromDb()
        {
            _messages.Clear();
            _messages.AddRange(await _dbContext.GetLogMessages());

            _channelsMonitored.Clear();

            foreach (MonitoredChannel channel in await _dbContext.GetMonitoredChannels())
            {
                _channelsMonitored.Add(channel.ChannelId, channel.IsMonitored);
            }
        }

        private async Task StoreMessage(MessageLog message)
        {
            _messages.Add(message);
            await _dbContext.InsertLogMessage(message);
        }

        //Check if dictionary contains the channel, and return the value if found
        //else return true
        public bool ShouldMonitorChannel(Channel ch) => !_channelsMonitored.ContainsKey(ch.Id) || _channelsMonitored[ch.Id];

        public async Task SetChannelMonitor(MonitoredChannel channel)
        {
            _channelsMonitored[channel.ChannelId] = channel.IsMonitored;
            await _dbContext.UpdateChannelMonitoring(channel);
        }

        public async void Analyze(Message message)
        {
            if (!ShouldMonitorChannel(message.Channel))
            {
                Log.Verbose("Skipping message from channel {channel}", message.Channel.Name);
                return;
            }

            float spamValue = Heuristics.CalculateSpamValue(message);

            Log.Logger.Information("Message from {user} had a spam rating of : {rating} ({heuristictype})", 
                message.User.Name, spamValue, Heuristics.AggregateMethod);

            List<string> heuristicLog = new List<string>();

            foreach (SpamHeuristic heuristic in Heuristics.Heuristics.Keys)
            {
                string heuristicName = heuristic.GetType().Name;
                float individualSpamValue = heuristic.CalculateSpamValue(message, false);
                float weight = Heuristics.Heuristics[heuristic];

                Log.Logger.Information("\t {heuristic}: {rating} ({weight}wt)", heuristicName, individualSpamValue, weight);

                heuristicLog.Add($"{heuristic}: {individualSpamValue} ({weight}wt)");
            }

            var toLog = new MessageLog
            {
                SenderName = message.User.Name,
                Message = message.Text,
                SentTime = message.Timestamp,
                SpamValue = spamValue,
                Channel = message.Channel.Name,
                Server = new DiscordServerInfo
                {
                    Name = message.Server.Name,
                    Icon = message.Server.IconUrl,
                    Id = message.Server.Id.ToString(),
                    UserCount = message.Server.UserCount
                },
                Details = heuristicLog,
                Formula = Heuristics.AggregateMethod.ToString()
            };

            foreach (SpamTrigger trigger in Triggers.OrderByDescending(d=>d.TriggerScore))
            {
                if (!trigger.ShouldTrigger(spamValue)) continue;

                Log.Logger.Information("Message from {user} caused a trigger: {trigger} ({messagescore}/{triggerscore})", 
                    message.User.Name, trigger.GetType().Name, spamValue, trigger.TriggerScore);

                toLog.Trigger = $"{trigger.GetType().Name}: out of {trigger.TriggerScore}";

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

            await StoreMessage(toLog);
        }
    }
}
