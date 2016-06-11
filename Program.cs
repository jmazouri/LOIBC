using System;
using LOIBC;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

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

            _bot = new LOIBCBot(botConfig);
            DoTasks();

            Console.WriteLine("LOIBC Connected. Press any key to quit.");
            Console.ReadKey();
        }

        static async void DoTasks()
        {
            await _bot.Connect();
        }
    }
}
