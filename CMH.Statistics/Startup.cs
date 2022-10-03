using System;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

using CMH.Statistics;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CMH.Statistics
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
