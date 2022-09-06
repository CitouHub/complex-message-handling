using Microsoft.AspNetCore.Mvc;

using CMH.Common.Enum;
using CMH.Data.Model;
using CMH.Data.Repository;

namespace CMH.Priority.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IMessageStatisticsRepository _messageStatisticsRepository;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;

        public StatisticsController(
            IMessageStatisticsRepository messageStatisticsRepository,
            IRuntimeStatisticsRepository runtimeStatisticsRepository)
        {
            _messageStatisticsRepository = messageStatisticsRepository;
            _runtimeStatisticsRepository = runtimeStatisticsRepository;
        }

        [HttpGet]
        [Route("messages/priority")]
        public Dictionary<short, MessageStatistics> GetPriorityMessagesStatistics()
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

        [HttpGet]
        [Route("runtime/reset")]
        public void ResetRuntimeStatistics()
        {
            _runtimeStatisticsRepository.ResetRuntimeStatistics();
        }
    }
}