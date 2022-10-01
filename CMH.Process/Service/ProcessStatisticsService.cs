using System;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Service
{
    public interface IProcessStatisticsService
    {
        Task QueuePendingHandledProcessMessage(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, DateTimeOffset executionStart);
    }

    public class ProcessStatisticsService : IProcessStatisticsService
    {
        private readonly ServiceBusClient _serviceBusClient;

        public ProcessStatisticsService(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        public async Task QueuePendingHandledProcessMessage(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, DateTimeOffset executionStart)
        {
            var sender = _serviceBusClient.CreateSender("handled_process_message");
            var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(
                new PendingHandledProcessMessage()
                {
                    ProcessChannel = processChannel,
                    MessageHandleStatus = messageHandleStatus,
                    Duration = (DateTimeOffset.UtcNow - executionStart).TotalMilliseconds
                }));
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}