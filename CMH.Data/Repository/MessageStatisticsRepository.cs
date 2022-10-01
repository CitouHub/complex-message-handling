using CMH.Common.Variable;
using CMH.Data.Infrastructure;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageHandeled(string priorityQueueName, MessageHandleStatus messageHandleStatus, double duration);
        void ProcessMessageHandeled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration);
        Dictionary<string, MessageStatistics> GetPriorityMessagesStatistics();
        Dictionary<ProcessChannel, MessageStatistics> GetProcessMessagesStatistics();
        void ResetPriorityMessagesStatistics();
        void ResetProcessMessagesStatistics();
    }

    public class MessageStatisticsRepository : IMessageStatisticsRepository
    {
        private Dictionary<string, MessageStatistics> _priorityMessagesStatistics = new();
        private Dictionary<ProcessChannel, MessageStatistics> _processMessagesStatistics = new();

        private void InitiatePriorityMessagesStatistics(string priorityQueueName)
        {
            if (string.IsNullOrEmpty(priorityQueueName) == false && !_priorityMessagesStatistics.ContainsKey(priorityQueueName))
            {
                _priorityMessagesStatistics[priorityQueueName] = new MessageStatistics();
            }
        }

        private void InitiateProcessMessagesStatistics(ProcessChannel processChannel)
        {
            if (!_processMessagesStatistics.ContainsKey(processChannel))
            {
                _processMessagesStatistics[processChannel] = new MessageStatistics();
            }
        }

        public void PriorityMessageHandeled(string priorityQueueName, MessageHandleStatus messageHandleStatus, double duration)
        {
            lock(Signals.MessageStatisticsRepository_PriorityLock)
            {
                InitiatePriorityMessagesStatistics(priorityQueueName);
                _priorityMessagesStatistics[priorityQueueName].MessageHandled(messageHandleStatus, duration);
            }
        }

        public void ProcessMessageHandeled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            lock(Signals.MessageStatisticsRepository_ProcessLock)
            {
                InitiateProcessMessagesStatistics(processChannel);
                _processMessagesStatistics[processChannel].MessageHandled(messageHandleStatus, duration);
            }
        }

        public Dictionary<string, MessageStatistics> GetPriorityMessagesStatistics()
        {
            return _priorityMessagesStatistics;
        }

        public Dictionary<ProcessChannel, MessageStatistics> GetProcessMessagesStatistics()
        {
            return _processMessagesStatistics;
        }

        public void ResetPriorityMessagesStatistics()
        {
            _priorityMessagesStatistics = new();
        }

        public void ResetProcessMessagesStatistics()
        {
            _processMessagesStatistics = new();
        }
    }
}
