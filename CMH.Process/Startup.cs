using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;

using Azure.Messaging.ServiceBus.Administration;

using CMH.Process;
using CMH.Infrastructure;

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

            builder.Services.AddAzureClients(_ =>
            {
                _.AddServiceBusClient(configuration.GetValue<string>("Values:ServiceBusConnection"));
                _.AddServiceBusAdministrationClient(configuration.GetValue<string>("Values:ServiceBusConnection"));
            });

            var processChannelDescriptions = configuration.GetSection("ProcessChannels").Get<List<string>>();

            if(processChannelDescriptions != null && processChannelDescriptions.Any())
            {
                var connectionString = configuration.GetValue<string>("Values:ServiceBusConnection");
                var serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);

                foreach(var channel in processChannelDescriptions)
                {
                    var values = channel.Split(';');
                    var processChannel = new ProcessChannel()
                    {
                        Name = values.FirstOrDefault(_ => _.StartsWith("Name")).Split('=')[1],
                        Tries = short.Parse(values.FirstOrDefault(_ => _.StartsWith("Tries")).Split('=')[1]) ,
                        InitialSleepTime = short.Parse(values.FirstOrDefault(_ => _.StartsWith("InitialSleepTime")).Split('=')[1]),
                        BackoffFactor = double.Parse(values.FirstOrDefault(_ => _.StartsWith("BackoffFactor")).Split('=')[1])
                    };

                    if(serviceBusAdministrationClient.QueueExistsAsync(processChannel.Name).Result == false)
                    {
                        serviceBusAdministrationClient.CreateQueueAsync(processChannel.Name).Wait();
                    }
                    
                    Variable.ProcessChannels.Add(processChannel);
                }
            }
        }
    }
}
