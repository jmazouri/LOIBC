using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace LOIBC
{
    public class LOIBCInterface
    {
        private IWebHost _webhost;

        public LOIBCInterface(string url, LOIBCBot bot)
        {
            _webhost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "WebInterface"))
                .UseUrls(url)
                .ConfigureServices(collection =>
                {
                    collection.AddSingleton(bot);
                })
                .UseStartup<InterfaceStartup>()
                .Build();

            Log.Information("Started web interface on {url}", url);

            _webhost.Start();
        }

        public void End()
        {
            _webhost.Dispose();
        }

        class InterfaceStartup
        {
            private LOIBCBot _bot;

            public InterfaceStartup(IHostingEnvironment env, LOIBCBot bot)
            {
                var builder = new ConfigurationBuilder();
                Configuration = builder.Build();

                _bot = bot;
            }

            public IConfigurationRoot Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                loggerFactory.AddSerilog();

                app.UseMvcWithDefaultRoute();
                app.UseStaticFiles();

                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();

            }
        }
    }
}
