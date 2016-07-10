using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LOIBC.SpamHeuristics;
using Microsoft.AspNetCore.Mvc;

namespace LOIBC.WebInterface.Controllers
{
    public class ApiController : Controller
    {
        private LOIBCBot _bot;

        public ApiController(LOIBCBot bot)
        {
            _bot = bot;
        }

        [HttpGet]
        public Dictionary<SpamHeuristic, float> Heuristics()
        {
            return _bot.RateMonitor.Heuristics.Heuristics;
        }

        [HttpGet]
        public string InviteLink()
        {
            return $"https://discordapp.com/oauth2/authorize?client_id={_bot.Config.ClientId}&scope=bot&permissions=8";
        }

        [HttpGet]
        public IEnumerable<DiscordServerInfo> ServerInfo()
        {
            return _bot.DiscordClient.Servers.Select(s => new DiscordServerInfo
            {
                Name = s.Name,
                UserCount = s.UserCount,
                Id = s.Id.ToString(),
                Icon = s.IconUrl,
                Channels = s.TextChannels
                    .ToDictionary(d=>d.Name, d=>_bot.RateMonitor.ShouldMonitorChannel(d))
            });
        }

        [HttpPost]
        public async Task UpdateServerChannels([FromBody] DiscordServerInfo server)
        {
            foreach (var channel in server.Channels)
            {
                ulong channelId = _bot.DiscordClient
                    .Servers
                    .FirstOrDefault(d => d.Id.ToString() == server.Id)
                    ?.TextChannels
                    .FirstOrDefault(d => d.Name == channel.Key)
                    ?.Id ?? 0;

                if (channelId > 0)
                {
                    await _bot.RateMonitor.SetChannelMonitor(new MonitoredChannel
                    {
                        ChannelId = channelId,
                        IsMonitored = channel.Value
                    });
                }
            }
        }

        [HttpGet]
        public IEnumerable<MessageLog> Logs()
        {
            return _bot.RateMonitor.MessageLog.OrderByDescending(d => d.SentTime);
        }
    }
}
