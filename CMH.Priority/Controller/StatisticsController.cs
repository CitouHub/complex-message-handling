using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Data.Repository;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly HttpClient _functionHttpClient;
        private readonly IMessageStatisticsRepository _messageStatisticsRepository;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;

        public StatisticsController(IHttpClientFactory httpClientFactory,
            IMessageStatisticsRepository messageStatisticsRepository,
            IRuntimeStatisticsRepository runtimeStatisticsRepository)
        {
            _functionHttpClient = httpClientFactory.CreateClient("Function");
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
        public Dictionary<ProcessChannel, MessageStatistics> GetProcessMessagesStatistics()
        {
            return _messageStatisticsRepository.GetProcessMessagesStatistics();
        }

        [HttpPut]
        [Route("messages/process/reset")]
        public async Task ResetProcessMessagesStatistics()
        {
            _messageStatisticsRepository.ResetProcessMessagesStatistics();
            await _functionHttpClient.PostAsync("statistics/reset", null);
        }

        [HttpGet]
        [Route("runtime")]
        public async Task<RuntimeStatistics> GetRuntimeStatistics()
        {
            var result = await _functionHttpClient.GetAsync("statistics");
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var functionRuntimeStatistics = JsonConvert.DeserializeObject<RuntimeStatistics>(content);
                var runtimeStatistics = _runtimeStatisticsRepository.GetRuntimeStatistics();

                runtimeStatistics.TotalMessagesProcessed = functionRuntimeStatistics?.TotalMessagesProcessed ?? 0;
                runtimeStatistics.TotalProcessDuration = functionRuntimeStatistics?.TotalProcessDuration ?? 0;
                runtimeStatistics.TotalMemoryUsage = functionRuntimeStatistics?.TotalMemoryUsage ?? 0;
                runtimeStatistics.SessionStart = functionRuntimeStatistics?.SessionStart;
                runtimeStatistics.SessionStop = functionRuntimeStatistics?.SessionStop;
                runtimeStatistics.MaxParallellTasks = functionRuntimeStatistics?.MaxParallellTasks ?? 0;
                runtimeStatistics.AvgParallellTasks = functionRuntimeStatistics?.AvgParallellTasks ?? 0;

                return runtimeStatistics;
            }

            return new RuntimeStatistics();
        }

        [HttpPut]
        [Route("runtime/reset")]
        public void ResetRuntimeStatistics()
        {
            _runtimeStatisticsRepository.ResetRuntimeStatistics();
        }

        [HttpPost]
        [Route("message/process/handled/{processChannel}/{processMessageStatus}/{duration}")]
        public void MessageHandledAsync(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            switch(messageHandleStatus)
            {
                case MessageHandleStatus.Completed:
                    _messageStatisticsRepository.ProcessMessageCompleted(processChannel, duration);
                    break;
                case MessageHandleStatus.Rescheduled:
                    _messageStatisticsRepository.ProcessMessageRescheduled(processChannel);
                    break;
                case MessageHandleStatus.Discarded:
                    _messageStatisticsRepository.ProcessMessageDiscarded(processChannel);
                    break;
            }
        }
    }
}