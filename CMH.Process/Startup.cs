using System;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

using CMH.Process;
using CMH.Process.Service;
using CMH.Common.Util;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CMH.Process
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddMemoryCache();

            builder.Services.AddAzureClients(_ =>
            {
                _.AddServiceBusClient(configuration.GetValue<string>("Values:ServiceBusConnection"));
                _.AddServiceBusAdministrationClient(configuration.GetValue<string>("Values:ServiceBusConnection"));
            });

            builder.Services.AddHttpClient("Service", _ =>
            {
                _.BaseAddress = new Uri($"{configuration.GetValue<string>("API:BaseUrl")}{configuration.GetValue<string>("API:Version")}/");
                _.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<IRepositoryService, RepositoryService>();
            builder.Services.AddScoped<IProcessStatisticsService, ProcessStatisticsService>();

            AwaitInitialization(configuration);
        }

        static void AwaitInitialization(IConfigurationRoot configuration)
        {
            var url = new Uri($"{configuration.GetValue<string>("API:BaseUrl")}{configuration.GetValue<string>("API:Version")}/status/ready");
            var httpClient = new HttpClient();

            while(true)
            {
                try
                {
                    var result = httpClient.GetAsync(url).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch { }

                Thread.Sleep(2000);
            }
        }
    }
}
