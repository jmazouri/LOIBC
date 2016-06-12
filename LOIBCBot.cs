using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace LOIBC
{
    public class LOIBCBot
    {
        private DiscordClient _client;
        private LOIBCConfig _config;
        private MessageRateMonitor _rateMonitor;

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
            Log.Logger.Verbose("{user}: {message}", e.Message.User.Name, e.Message.Text);
            _rateMonitor.Analyze(e.Message);
        }
    }
}
