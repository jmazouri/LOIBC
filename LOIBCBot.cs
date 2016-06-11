using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;

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

            _rateMonitor = new MessageRateMonitor();
        }

        public async Task Connect()
        {
            await _client.Connect(_config.BotKey);
            _client.MessageReceived += MessageReceived;
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"{e.Message.User.Name}: {e.Message.Text}");
            _rateMonitor.Add(e.Message);
        }
    }
}
