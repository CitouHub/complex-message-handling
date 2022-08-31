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

namespace CMH.Function
{
    public class ProcessFunction
    {
        private readonly ServiceBusClient _serviceBusClient;

        public ProcessFunction(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        [FunctionName("ProcessChannel_Default")]
        public async Task ProcessChannel_Default([ServiceBusTrigger("ProcessChannel_Default", Connection = "ServiceBusConnection", AutoCompleteMessages = false)] ServiceBusReceivedMessage message, 
            ILogger log, ExecutionContext executionContext)
        {
            var processChannel = GetProcessChannel(executionContext.FunctionName);
            var success = await HandleJobMessageAsync(message.Body.ToString());
            await HandleResult(success, processChannel, message);
        }

        [FunctionName("ProcessChannel_Slow1")]
        public async Task ProcessChannel_Slow1([ServiceBusTrigger("ProcessChannel_Slow1", Connection = "ServiceBusConnection", AutoCompleteMessages = false)] ServiceBusReceivedMessage message, 
            ILogger log, ExecutionContext executionContext)
        {
            var processChannel = GetProcessChannel(executionContext.FunctionName);
            var success = await HandleJobMessageAsync(message.Body.ToString());
            await HandleResult(success, processChannel, message);
        }

        [FunctionName("ProcessChannel_Slow2")]
        public async Task ProcessChannel_Slow2([ServiceBusTrigger("ProcessChannel_Slow2", Connection = "ServiceBusConnection", AutoCompleteMessages = false)] ServiceBusReceivedMessage message, 
            ILogger log, ExecutionContext executionContext)
        {
            var processChannel = GetProcessChannel(executionContext.FunctionName);
            var success = await HandleJobMessageAsync(message.Body.ToString());
            await HandleResult(success, processChannel, message);
        }

        [FunctionName("ProcessChannel_Slow3")]
        public async Task ProcessChannel_Slow3([ServiceBusTrigger("ProcessChannel_Slow3", Connection = "ServiceBusConnection", AutoCompleteMessages = false)] ServiceBusReceivedMessage message, 
            ILogger log, ExecutionContext executionContext)
        {
            var processChannel = GetProcessChannel(executionContext.FunctionName);
            var success = await HandleJobMessageAsync(message.Body.ToString());
            await HandleResult(success, processChannel, message);
        }

        private static ProcessChannel GetProcessChannel(string functionName)
        {
            return Variable.ProcessChannels.FirstOrDefault(_ => _.Name == functionName);
        }

        private async Task HandleResult(bool success, ProcessChannel processChannel, ServiceBusReceivedMessage message)
        {
            if (success)
            {
                var receiver = _serviceBusClient.CreateReceiver(processChannel.Name);
                await receiver.CompleteMessageAsync(message);
            } 
            else
            {
                var offset = processChannel.InitialSleepTime * Math.Pow(processChannel.BackoffSleepTimeFactor, message.DeliveryCount);
                var sender = _serviceBusClient.CreateSender(processChannel.Name);
                await sender.ScheduleMessageAsync(new ServiceBusMessage(message), DateTimeOffset.UtcNow.AddMilliseconds(offset));
            }
        }

        private static async Task<bool> HandleJobMessageAsync(string message)
        {
            var jobMessage = JsonConvert.DeserializeObject<JobMessage>(message);
            var dataSource = GetDataSource(jobMessage.DataSourceId);
            return await dataSource.DoWork();
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
