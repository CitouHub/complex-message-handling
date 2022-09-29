using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Data.Repository;
using CMH.Common.Util;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly HttpClient _functionHttpClient;
        private readonly ICacheManager _cacheManager;
        private readonly IMessageStatisticsRepository _messageStatisticsRepository;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;

        public StatisticsController(IHttpClientFactory httpClientFactory,
            ICacheManager cacheManager,
            IMessageStatisticsRepository messageStatisticsRepository,
            IRuntimeStatisticsRepository runtimeStatisticsRepository)
        {
            _functionHttpClient = httpClientFactory.CreateClient("Function");
            _cacheManager = cacheManager;
            _messageStatisticsRepository = messageStatisticsRepository;
            _runtimeStatisticsRepository = runtimeStatisticsRepository;
        }

        [HttpGet]
        [Route("messages/priority")]
        public Dictionary<string, MessageStatistics> GetPriorityMessagesStatistics()
        {
            return _messageStatisticsRepository.GetPriorityMessagesStatistics();
        }

        [HttpPut]
        [Route("messages/priority/reset")]
        public void ResetPriorityMessagesStatistics()
        {
            _messageStatisticsRepository.ResetPriorityMessagesStatistics();
        }

        [HttpGet]
        [Route("messages/process")]
        public async Task<Dictionary<ProcessChannel, MessageStatistics>> GetProcessMessagesStatistics()
        {
            var cacheKey = "GetProcessMessagesStatistics";
            Dictionary<ProcessChannel, MessageStatistics>? messageStatistics = null;
            try
            {
                _functionHttpClient.BaseAddress = new Uri(string.Format(_functionHttpClient.BaseAddress?.ToString() ?? "", "statistics"));
                var result = await _functionHttpClient.GetAsync("");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    messageStatistics = JsonConvert.DeserializeObject<Dictionary<ProcessChannel, MessageStatistics>>(content);
                    _cacheManager.Set(cacheKey, messageStatistics);
                }
            } 
            catch
            {
                messageStatistics = _cacheManager.Get<Dictionary<ProcessChannel, MessageStatistics>>(cacheKey);
            }

            return messageStatistics ?? new Dictionary<ProcessChannel, MessageStatistics>();
        }

        [HttpPut]
        [Route("messages/process/reset")]
        public async Task ResetProcessMessagesStatistics()
        {
            try
            {
                _functionHttpClient.BaseAddress = new Uri(string.Format(_functionHttpClient.BaseAddress?.ToString() ?? "", "statistics/reset"));
                await _functionHttpClient.PostAsync("", null);
            } catch { }
        }

        [HttpGet]
        [Route("runtime")]
        public RuntimeStatistics GetRuntimeStatistics()
        {
            return _runtimeStatisticsRepository.GetRuntimeStatistics();
        }

        [HttpPut]
        [Route("runtime/reset")]
        public void ResetRuntimeStatistics()
        {
            _runtimeStatisticsRepository.ResetRuntimeStatistics();
        }
    }
}