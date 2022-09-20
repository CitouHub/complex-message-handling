using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Service
{
    public interface IRepositoryService
    {
        Task<DataSource> GetDataSourceAsync(short dataSourceId);
        Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel);
        Task MessageHandledAsync(ProcessChannel processChannel, MessageHandleStatus processMessageStatus, DateTimeOffset enqueueTime);
    }

    public class RepositoryService : IRepositoryService
    {
        private readonly HttpClient _httpClient;

        public RepositoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RepositoryService");
        }

        public async Task<DataSource> GetDataSourceAsync(short dataSourceId)
        {
            var result = await _httpClient.GetAsync($"datasource/{dataSourceId}");
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DataSource>(content);
            }

            return null;
        }

        public async Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel)
        {
            var result = await _httpClient.GetAsync($"processchannelpolicy/{processChannel}");
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ProcessChannelPolicy>(content);
            }

            return null;
        }

        public async Task MessageHandledAsync(ProcessChannel processChannel, MessageHandleStatus processMessageStatus, DateTimeOffset enqueueTime)
        {
            var duration = (DateTimeOffset.UtcNow - enqueueTime).TotalMilliseconds;
            await _httpClient.PostAsync($"statistics/message/process/handled/{processChannel}/{processMessageStatus}/{Math.Round(duration)}", null);
        }
    }
}
