using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Process.Infrastructure;

namespace CMH.Process.Service
{
    public interface IProcessStatisticsService
    {
        void AddPendingHandeledProcessMessage(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration);
        Task<int> FlushPendingHandeledProcessMessagesAsync();
    }

    public class ProcessStatisticsService : IProcessStatisticsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        private readonly string CacheKey = "PendingHandeledProcessMessage";

        public ProcessStatisticsService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClient = httpClientFactory.CreateClient("Service");
            _memoryCache = memoryCache;
        }

        public void AddPendingHandeledProcessMessage(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            lock (Signal.PendingHandeledProcessMessageLock)
            {
                var pendingHandledProcessMessages = _memoryCache.Get<List<PendingHandledProcessMessage>>(CacheKey);
                pendingHandledProcessMessages ??= new();

                pendingHandledProcessMessages.Add(new PendingHandledProcessMessage()
                {
                    ProcessChannel = processChannel,
                    MessageHandleStatus = messageHandleStatus,
                    Duration = duration
                });

                _memoryCache.Set(CacheKey, pendingHandledProcessMessages);
            }
        }

        public async Task<int> FlushPendingHandeledProcessMessagesAsync()
        {
            List<PendingHandledProcessMessage> messagesToFlush = new();
            lock (Signal.PendingHandeledProcessMessageLock)
            {
                messagesToFlush = _memoryCache.Get<List<PendingHandledProcessMessage>>(CacheKey) ?? new();
                _memoryCache.Remove(CacheKey);
            }

            if(messagesToFlush.Count > 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(messagesToFlush), Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"statistics/messages/process", content);
            }

            return messagesToFlush.Count;
        }
    }
}