using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Service
{
    public interface IProcessStatisticsService
    {
        Task ProcessMessageHandled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration);
    }

    public class ProcessStatisticsService : IProcessStatisticsService
    {
        private readonly HttpClient _httpClient;

        public ProcessStatisticsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Service");
        }

        public async Task ProcessMessageHandled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            var messages = new List<PendingHandledProcessMessage>()
            {
                new PendingHandledProcessMessage()
                {
                    ProcessChannel = processChannel,
                    MessageHandleStatus = messageHandleStatus,
                    Duration = duration
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(messages), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"statistics/messages/process", content);
        }
    }
}