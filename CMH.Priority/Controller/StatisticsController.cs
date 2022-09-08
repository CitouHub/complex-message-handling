using Microsoft.AspNetCore.Mvc;

using CMH.Common.Variable;
using CMH.Data.Model;
using CMH.Data.Repository;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
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

        [HttpPost]
        [Route("runtime/process/finished/{duration}")]
        public void ProcessFinishedAsync(double duration)
        {
            _runtimeStatisticsRepository.MessageProcessingFinished(duration);
        }
    }
}