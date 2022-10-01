using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

using CMH.Data.Model;
using CMH.Process.Service;

namespace CMH.Statistics
{
    public class HandledProcessMessagesFunction
    {
        private readonly IProcessStatisticsService _processStatisticsService;

        public HandledProcessMessagesFunction(IProcessStatisticsService processStatisticsService)
        {
            _processStatisticsService = processStatisticsService;
        }

        [FunctionName("HandledProcessMessages")]
        public async Task HandledProcessMessage([ServiceBusTrigger("handled_process_message", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ILogger log, ExecutionContext executionContext)
        {
            var pendingHandledProcessMessage = JsonConvert.DeserializeObject<PendingHandledProcessMessage>(message.Body.ToString());
            await _processStatisticsService.ProcessMessageHandled(
                pendingHandledProcessMessage.ProcessChannel,
                pendingHandledProcessMessage.MessageHandleStatus,
                pendingHandledProcessMessage.Duration);
        }
    }
}
