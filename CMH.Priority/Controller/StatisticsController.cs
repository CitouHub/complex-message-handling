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

        [HttpPost]
        [Route("messages/process")]
        public void ProcessMessagesHandled([FromBody] List<PendingHandledProcessMessage> pendingHandledProcessMessages)
        {
            foreach(var pendingHandledProcessMessage in pendingHandledProcessMessages)
            {
                _messageStatisticsRepository.ProcessMessageHandeled(
                    pendingHandledProcessMessage.ProcessChannel,
                    pendingHandledProcessMessage.MessageHandleStatus,
                    pendingHandledProcessMessage.Duration);
            }
        }

        [HttpGet]
        [Route("messages/process")]
        public Dictionary<ProcessChannel, MessageStatistics> GetProcessMessagesStatistics()
        {
            return _messageStatisticsRepository.GetProcessMessagesStatistics();
        }

        [HttpPut]
        [Route("messages/process/reset")]
        public void ResetProcessMessagesStatistics()
        {
            _messageStatisticsRepository.ResetProcessMessagesStatistics();
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