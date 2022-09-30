using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Service
{
    public interface IRepositoryService
    {
        Task<DataSource> GetDataSourceAsync(short dataSourceId);
        Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel);
        void ResetCache();
    }

    public class RepositoryService : IRepositoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        private readonly string GetDataSourceAsync_CacheKey = "GetDataSourceAsync";
        private readonly string GetProcessChannelPolicyAsync_CacheKey = "GetProcessChannelPolicyAsync";

        public RepositoryService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClient = httpClientFactory.CreateClient("Service");
            _memoryCache = memoryCache;
        }

        public async Task<DataSource> GetDataSourceAsync(short dataSourceId)
        {
            var dataSources = _memoryCache.Get<Dictionary<short, DataSource>>(GetDataSourceAsync_CacheKey);
            if (dataSources != null && dataSources.ContainsKey(dataSourceId))
            {
                return dataSources[dataSourceId];
            } 
            else
            {
                var result = await _httpClient.GetAsync($"datasource/{dataSourceId}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var dataSource = JsonConvert.DeserializeObject<DataSource>(content);
                    lock (_memoryCache)
                    {
                        dataSources = _memoryCache.Get<Dictionary<short, DataSource>>(GetDataSourceAsync_CacheKey);
                        dataSources ??= new Dictionary<short, DataSource>();
                        dataSources[dataSourceId] = dataSource;
                        _memoryCache.Set(GetDataSourceAsync_CacheKey, dataSources);
                    }

                    return dataSource;
                }
            }
            
            return null;
        }

        public async Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel)
        {
            var processChannelPolicies = _memoryCache.Get<Dictionary<ProcessChannel, ProcessChannelPolicy>>(GetProcessChannelPolicyAsync_CacheKey);
            if (processChannelPolicies != null && processChannelPolicies.ContainsKey(processChannel))
            {
                return processChannelPolicies[processChannel];
            }
            else
            {
                var result = await _httpClient.GetAsync($"processchannelpolicy/{processChannel}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var processChannelPolicy = JsonConvert.DeserializeObject<ProcessChannelPolicy>(content);
                    lock (_memoryCache)
                    {
                        processChannelPolicies = _memoryCache.Get<Dictionary<ProcessChannel, ProcessChannelPolicy>>(GetProcessChannelPolicyAsync_CacheKey);
                        processChannelPolicies ??= new Dictionary<ProcessChannel, ProcessChannelPolicy>();
                        processChannelPolicies[processChannel] = processChannelPolicy;
                        _memoryCache.Set(GetProcessChannelPolicyAsync_CacheKey, processChannelPolicies);
                    }

                    return processChannelPolicy;
                }
            }

            return null;
        }

        public void ResetCache()
        {
            lock(_memoryCache)
            {
                _memoryCache.Remove(GetDataSourceAsync_CacheKey);
                _memoryCache.Remove(GetProcessChannelPolicyAsync_CacheKey);
            }
        }
    }
}
