using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

using CMH.Common.Message;
using CMH.Common.Util;
using CMH.Common.Extenstion;
using CMH.Process.Extension;
using CMH.Common.Variable;
using CMH.Process.Service;

namespace CMH.Function
{
    public class ProcessFunction
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IRepositoryService _repositoryService;
        private readonly IProcessStatisticsService _processStatisticsService;

        public ProcessFunction(ServiceBusClient serviceBusClient, IRepositoryService repositoryService, IProcessStatisticsService processStatisticsService)
        {
            _serviceBusClient = serviceBusClient;
            _repositoryService = repositoryService;
            _processStatisticsService = processStatisticsService;
        }

        [FunctionName("ResetProcess")]
        public IActionResult ResetStatistics([HttpTrigger(AuthorizationLevel.Function, "post", Route = "process/reset")] HttpRequest req, ILogger log)
        {
            _repositoryService.ResetCache();
            return new OkResult();
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
            try
            {
                var executionStart = DateTimeOffset.UtcNow;
                var processChannel = Enum.Parse<ProcessChannel>(functionName.Split('_')[1]);
                var success = await HandleJobMessageAsync(message.Body.ToString());
                var messageHandleStatus = await HandleResult(success, processChannel, message, log);
                _processStatisticsService.AddPendingHandeledProcessMessage(
                    processChannel, messageHandleStatus, (DateTimeOffset.UtcNow - executionStart).TotalMilliseconds);
            } 
            catch(Exception e)
            {
                log.LogError($"{e.Message}\n{e.StackTrace}", e);
                throw;
            } 
        }

        private async Task<bool> HandleJobMessageAsync(string message)
        {
            var jobMessage = JsonConvert.DeserializeObject<JobMessage>(message);
            var dataSource = await _repositoryService.GetDataSourceAsync(jobMessage.DataSourceId);
            if (dataSource == null)
            {
                throw new Exception($"Unknown data source {jobMessage.DataSourceId}");
            }

            return await dataSource.DoWork();
        }

        private async Task<MessageHandleStatus> HandleResult(bool success, ProcessChannel processChannel, ServiceBusReceivedMessage message, ILogger log)
        {
            if (!success)
            {
                var processChannelPolicy = await _repositoryService.GetProcessChannelPolicyAsync(processChannel);
                var tries = (int)message.ApplicationProperties["Tries"];
                if(tries < processChannelPolicy.Tries)
                {
                    var sleepTime = BackoffCalculator.CalculateProcessRescheduleSleepTime(
                        processChannelPolicy.InitialSleepTime,
                        processChannelPolicy.BackoffFactor,
                        (int)message.ApplicationProperties["Tries"]);
                    var scheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(sleepTime);

                    log.LogInformation($"Processing message failed, new attempted rescheduled in {sleepTime} s, at {scheduledEnqueueTime:mm:hh:ss}");

                    var sender = _serviceBusClient.CreateSender($"{Queue.ProcessQueuePrefix}{processChannelPolicy.Name}");
                    await sender.RescheduleMessageAsync(message, scheduledEnqueueTime);
                    return MessageHandleStatus.Rescheduled;
                } 
                else
                {
                    log.LogWarning($"Unable to process message {message.MessageId} {message.Body} due to too many tries");
                    return MessageHandleStatus.Discarded;
                }
            }
            else
            {
                return MessageHandleStatus.Completed;
            }
        }
    }
}
