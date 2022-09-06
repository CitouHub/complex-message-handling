using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

using Azure.Messaging.ServiceBus.Administration;

using CMH.Process;
using CMH.Data.Repository;
using CMH.Data.Model;
using CMH.Common.Enum;
using System.Globalization;

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

            builder.Services.AddSingleton<IRuntimeStatisticsRepository, RuntimeStatisticsRepository>();
            builder.Services.AddSingleton<IProcessChannelPolicyRepository, ProcessChannelPolicyRepository>();

            InitiateProcessChannels(builder, configuration);
        }

        private static void InitiateProcessChannels(IFunctionsHostBuilder builder, IConfigurationRoot configuration)
        {
            var provider = builder.Services.BuildServiceProvider();
            var processChannelPolicyRepository = provider.GetRequiredService<IProcessChannelPolicyRepository>();

            var processChannelPolicies = configuration.GetSection("ProcessChannelPolicies").Get<List<string>>();
            if (processChannelPolicies != null && processChannelPolicies.Any())
            {
                var connectionString = configuration.GetValue<string>("Values:ServiceBusConnection");
                var serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);

                foreach (var policy in processChannelPolicies)
                {
                    var values = policy.Split(';');
                    var processChannelPolicy = new ProcessChannelPolicy()
                    {
                        Name = Enum.Parse<ProcessChannel>(values.FirstOrDefault(_ => _.StartsWith("Name=")).Split('=')[1]).ToString(),
                        Tries = short.Parse(values.FirstOrDefault(_ => _.StartsWith("Tries=")).Split('=')[1]),
                        InitialSleepTime = short.Parse(values.FirstOrDefault(_ => _.StartsWith("InitialSleepTime=")).Split('=')[1]),
                        BackoffFactor = double.Parse(values.FirstOrDefault(_ => _.StartsWith("BackoffFactor=")).Split('=')[1], CultureInfo.InvariantCulture)
                    };

                    var processChannelQueueName = $"ProcessChannel_{processChannelPolicy.Name}";
                    if (serviceBusAdministrationClient.QueueExistsAsync(processChannelQueueName).Result == false)
                    {
                        serviceBusAdministrationClient.CreateQueueAsync(processChannelQueueName).Wait();
                    }

                    processChannelPolicyRepository.Add(processChannelPolicy);
                }
            }
        }
    }
}
