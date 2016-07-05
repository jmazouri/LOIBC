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
        public DiscordClient DiscordClient { get; private set; }
        public readonly LOIBCConfig Config;
        public MessageRateMonitor RateMonitor { get; private set; }

        public IEnumerable<SpamHeuristic> SpamHeuristics => RateMonitor.Heuristics.Heuristics.Select(d=>d.Key);

        public LOIBCBot(LOIBCConfig config)
        {
            Config = config;

            var discordConfig = new DiscordConfigBuilder
            {
                AppName = "LOIBCBot"
            }.Build();

            DiscordClient = new DiscordClient(discordConfig);
        }

        public async Task Connect()
        {
            DiscordClient.MessageReceived += MessageReceived;
            await DiscordClient.Connect(Config.BotKey);

            RateMonitor = new MessageRateMonitor(DiscordClient);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.Id == DiscordClient.CurrentUser.Id) { return; }

            Log.Logger.Verbose("{user}: {message}", e.Message.User.Name, e.Message.Text);
            RateMonitor.Analyze(e.Message);
        }
    }
}
