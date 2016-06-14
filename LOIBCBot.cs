using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace LOIBC
{
    public class LOIBCBot
    {
        private DiscordClient _client;
        private LOIBCConfig _config;
        private MessageRateMonitor _rateMonitor;

        public IEnumerable<SpamHeuristic> SpamHeuristics => _rateMonitor.Heuristics.Heuristics.Select(d=>d.Key);

        public LOIBCBot(LOIBCConfig config)
        {
            _config = config;

            var discordConfig = new DiscordConfigBuilder
            {
                AppName = "LOIBCBot"
            }.Build();

            _client = new DiscordClient(discordConfig);
        }

        public async Task Connect()
        {
            _client.MessageReceived += MessageReceived;
            await _client.Connect(_config.BotKey);

            _rateMonitor = new MessageRateMonitor(_client);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.Id == _client.CurrentUser.Id) { return; }

            Log.Logger.Verbose("{user}: {message}", e.Message.User.Name, e.Message.Text);
            _rateMonitor.Analyze(e.Message);
        }
    }
}
