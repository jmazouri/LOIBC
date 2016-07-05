using System;
using LOIBC;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ConsoleApplication
{
    public class Program
    {
        private static LOIBCBot _bot;

        public static void Main(string[] args)
        {
            LOIBCConfig botConfig = new LOIBCConfig
            (
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build()
            );

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole(LogEventLevel.Verbose)
                .WriteTo.File(botConfig.LogPath, LogEventLevel.Information)
                .CreateLogger();

            _bot = new LOIBCBot(botConfig);
            _bot.Connect().Wait();

            var webInterface = new LOIBCInterface("http://localhost:8080", _bot);

            Console.WriteLine("LOIBC Connected. Press any key to quit.");
            Console.ReadKey();
        }
    }
}
