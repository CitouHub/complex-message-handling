using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

using CMH.Common.Message;
using CMH.Common.Util;
using CMH.Common.Extenstion;
using CMH.Data.Repository;
using CMH.Process.Extension;
using CMH.Common.Enum;

namespace CMH.Function
{
    public class ProcessFunction
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;
        private readonly IDataSourceRepository _dataSourceRepository;
        private readonly IProcessChannelPolicyRepository _processChannelPolicyRepository;

        public ProcessFunction(ServiceBusClient serviceBusClient,
            IRuntimeStatisticsRepository runtimeStatisticsRepository,
            IDataSourceRepository dataSourceRepository,
            IProcessChannelPolicyRepository processChannelPolicyRepository)
        {
            _serviceBusClient = serviceBusClient;
            _runtimeStatisticsRepository = runtimeStatisticsRepository;
            _dataSourceRepository = dataSourceRepository;
            _processChannelPolicyRepository = processChannelPolicyRepository;
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
            await HandleResult(success, functionName.Split('_')[1], message, log);
            _runtimeStatisticsRepository.MessageProcessingFinished((DateTimeOffset.UtcNow - executionStart).TotalMilliseconds);
        }

        private async Task<bool> HandleJobMessageAsync(string message)
        {
            var jobMessage = JsonConvert.DeserializeObject<JobMessage>(message);
            var dataSource = _dataSourceRepository.Get(jobMessage.DataSourceId);
            if (dataSource == null)
            {
                throw new Exception("Unknown datasource");
            }

            return await dataSource.DoWork();
        }

        private async Task HandleResult(bool success, string processChannelName, ServiceBusReceivedMessage message, ILogger log)
        {
            if (!success)
            {
                var processChannelPolicy = _processChannelPolicyRepository.Get(Enum.Parse<ProcessChannel>(processChannelName));
                var tries = (int)message.ApplicationProperties["Tries"];
                if(tries < processChannelPolicy.Tries)
                {
                    var sleepTime = BackoffCalculator.CalculateProcessRescheduleSleepTime(
                        processChannelPolicy.InitialSleepTime,
                        processChannelPolicy.BackoffFactor,
                        (int)message.ApplicationProperties["Tries"]);
                    var scheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(sleepTime);

                    log.LogInformation($"Processing message failed, new attempted rescheduled in {sleepTime} s, at {scheduledEnqueueTime:mm:hh:ss}");

                    var sender = _serviceBusClient.CreateSender(processChannelPolicy.Name);
                    await sender.RescheduleMessageAsync(message, scheduledEnqueueTime);
                } 
                else
                {
                    log.LogWarning($"Unable to process message {message.MessageId} {message.Body} due to too many tries");
                }
            }
        }
    }
}
