using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Process.Util;
using CMH.Common.Util;

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
        private readonly ICacheManager _cacheManager;

        public RepositoryService(IHttpClientFactory httpClientFactory, ICacheManager cacheManager)
        {
            _httpClient = httpClientFactory.CreateClient("RepositoryService");
            _cacheManager = cacheManager;
        }

        public async Task<DataSource> GetDataSourceAsync(short dataSourceId)
        {
            var cacheKey = $"GetDataSourceAsync({dataSourceId})";
            var dataSource = _cacheManager.Get<DataSource>(cacheKey);
            if (dataSource != null)
            {
                return dataSource;
            } 
            else
            {
                var result = await _httpClient.GetAsync($"datasource/{dataSourceId}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    dataSource = JsonConvert.DeserializeObject<DataSource>(content);
                    _cacheManager.Set(cacheKey, dataSource);
                    return dataSource;
                }
            }
            
            return null;
        }

        public async Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel)
        {
            var cacheKey = $"GetProcessChannelPolicyAsync({processChannel})";
            var processChannelPolicy = _cacheManager.Get<ProcessChannelPolicy>(cacheKey);
            if (processChannelPolicy != null)
            {
                return processChannelPolicy;
            }
            else
            {
                var result = await _httpClient.GetAsync($"processchannelpolicy/{processChannel}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    processChannelPolicy = JsonConvert.DeserializeObject<ProcessChannelPolicy>(content);
                    _cacheManager.Set(cacheKey, processChannelPolicy);
                    return processChannelPolicy;
                }
            }

            return null;
        }

        public void ResetCache()
        {
            _cacheManager.Clear();
        }
    }
}
