using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

using CMH.Common.Message;
using CMH.Process.DataSourceMock;
using CMH.Process;
using CMH.Common.Util;
using CMH.Common.Extenstion;
using CMH.Common.Repository;

namespace CMH.Function
{
    public class ProcessFunction
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;

        public ProcessFunction(ServiceBusClient serviceBusClient,
            IRuntimeStatisticsRepository runtimeStatisticsRepository)
        {
            _serviceBusClient = serviceBusClient;
            _runtimeStatisticsRepository = runtimeStatisticsRepository;
        }

        [FunctionName("ProcessChannel_Default")]
        public async Task ProcessChannel_Default([ServiceBusTrigger("ProcessChannel_Default", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, 
            ILogger log, ExecutionContext executionContext)
        {
            await ProcessMessageAsync(message, log, executionContext.FunctionName);
        }

        [FunctionName("ProcessChannel_Slow1")]
        public async Task ProcessChannel_Slow1([ServiceBusTrigger("ProcessChannel_Slow1", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ILogger log, ExecutionContext executionContext)
        {
            await ProcessMessageAsync(message, log, executionContext.FunctionName);
        }

        [FunctionName("ProcessChannel_Slow2")]
        public async Task ProcessChannel_Slow2([ServiceBusTrigger("ProcessChannel_Slow2", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ILogger log, ExecutionContext executionContext)
        {
            await ProcessMessageAsync(message, log, executionContext.FunctionName);
        }

        [FunctionName("ProcessChannel_Slow3")]
        public async Task ProcessChannel_Slow3([ServiceBusTrigger("ProcessChannel_Slow3", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ILogger log, ExecutionContext executionContext)
        {
            await ProcessMessageAsync(message, log, executionContext.FunctionName);
        }

        public async Task ProcessMessageAsync(ServiceBusReceivedMessage message, ILogger log, string functionName)
        {
            var executionStart = DateTimeOffset.UtcNow;
            var success = await HandleJobMessageAsync(message.Body.ToString());
            await HandleResult(success, functionName, message, log);
            _runtimeStatisticsRepository.MessageProcessingFinished((DateTimeOffset.UtcNow - executionStart).TotalMilliseconds);
        }

        private static async Task<bool> HandleJobMessageAsync(string message)
        {
            var jobMessage = JsonConvert.DeserializeObject<JobMessage>(message);
            var dataSource = GetDataSource(jobMessage.DataSourceId);
            return await dataSource.DoWork();
        }

        private async Task HandleResult(bool success, string functionName, ServiceBusReceivedMessage message, ILogger log)
        {
            if (!success)
            {
                var processChannel = Variable.ProcessChannels.FirstOrDefault(_ => _.Name == functionName);
                var tries = (int)message.ApplicationProperties["Tries"];
                if(tries < processChannel.Tries)
                {
                    var sleepTime = BackoffCalculator.CalculateProcessRescheduleSleepTime(
                        processChannel.InitialSleepTime,
                        processChannel.BackoffFactor,
                        (int)message.ApplicationProperties["Tries"]);
                    var scheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(sleepTime);

                    log.LogInformation($"Processing message failed, new attempted rescheduled in {sleepTime} s, at {scheduledEnqueueTime:mm:hh:ss}");

                    var sender = _serviceBusClient.CreateSender(processChannel.Name);
                    await sender.RescheduleMessageAsync(message, scheduledEnqueueTime);
                } 
                else
                {
                    log.LogWarning($"Unable to process message {message.MessageId} {message.Body} due to too many tries");
                }
            }
        }

        private static IDataSource GetDataSource(short? dataSourceId)
        {
            return dataSourceId switch
            {
                1 => new DataSource1(),
                2 => new DataSource2(),
                3 => new DataSource3(),
                4 => new DataSource4(),
                5 => new DataSource5(),
                6 => new DataSource6(),
                7 => new DataSource7(),
                8 => new DataSource8(),
                9 => new DataSource9(),
                10 => new DataSource10(),
                _ => new DataSource1(),
            };
        }
    }
}
