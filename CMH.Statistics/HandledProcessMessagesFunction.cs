using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

using CMH.Common.Variable;
using CMH.Data.Model;
using System;

namespace CMH.Statistics
{
    public class HandledProcessMessagesFunction
    {
        private readonly HttpClient _httpClient;

        public HandledProcessMessagesFunction(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Service");
        }

        [FunctionName("HandledProcessMessages")]
        public async Task HandledProcessMessage([ServiceBusTrigger("handled_process_message", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ILogger log, ExecutionContext executionContext)
        {
            var pendingHandledProcessMessage = JsonConvert.DeserializeObject<PendingHandledProcessMessage>(message.Body.ToString());
            await ProcessMessageHandled(
                pendingHandledProcessMessage.ProcessChannel,
                pendingHandledProcessMessage.MessageHandleStatus,
                pendingHandledProcessMessage.StartTime,
                pendingHandledProcessMessage.StopTime);
        }

        private async Task ProcessMessageHandled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, DateTimeOffset startTime, DateTimeOffset stopTime)
        {
            var message = new PendingHandledProcessMessage()
            {
                ProcessChannel = processChannel,
                MessageHandleStatus = messageHandleStatus,
                StartTime = startTime,
                StopTime = stopTime
            };
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"statistics/messages/process", content);
        }
    }
}