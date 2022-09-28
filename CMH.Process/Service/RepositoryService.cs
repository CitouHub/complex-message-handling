using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Process.Util;

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

        public RepositoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RepositoryService");
        }

        public async Task<DataSource> GetDataSourceAsync(short dataSourceId)
        {
            if(Cache.DataSource.ContainsKey(dataSourceId))
            {
                return Cache.DataSource[dataSourceId];
            } 
            else
            {
                var result = await _httpClient.GetAsync($"datasource/{dataSourceId}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var dataSource = JsonConvert.DeserializeObject<DataSource>(content);
                    lock(Cache.DataSource)
                    {
                        if (dataSource != null && Cache.DataSource.ContainsKey(dataSourceId) == false)
                        {
                            Cache.DataSource[dataSourceId] = dataSource;
                        }
                    }
                    return dataSource;
                }
            }
            
            return null;
        }

        public async Task<ProcessChannelPolicy> GetProcessChannelPolicyAsync(ProcessChannel processChannel)
        {
            if (Cache.ProcessChannelPolicy.ContainsKey(processChannel))
            {
                return Cache.ProcessChannelPolicy[processChannel];
            }
            else
            {
                var result = await _httpClient.GetAsync($"processchannelpolicy/{processChannel}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var processChannelPolicy = JsonConvert.DeserializeObject<ProcessChannelPolicy>(content);
                    lock(Cache.ProcessChannelPolicy)
                    {
                        if (processChannelPolicy != null && Cache.ProcessChannelPolicy.ContainsKey(processChannel) == false)
                        {
                            Cache.ProcessChannelPolicy[processChannel] = processChannelPolicy;
                        }
                    }
                    return processChannelPolicy;
                }
            }

            return null;
        }

        public void ResetCache()
        {
            Cache.DataSource = new();
            Cache.ProcessChannelPolicy = new();
        }
    }
}
