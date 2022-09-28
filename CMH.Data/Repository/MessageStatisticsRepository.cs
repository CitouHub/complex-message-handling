using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageHandeled(string priorityQueueName, MessageHandleStatus messageHandleStatus, double duration);
        Dictionary<string, MessageStatistics> GetPriorityMessagesStatistics();
        void ResetPriorityMessagesStatistics();
    }

    public class MessageStatisticsRepository : IMessageStatisticsRepository
    {
        private Dictionary<string, MessageStatistics> _priorityMessagesStatistics = new();

        private void InitiatePriorityMessagesStatistics(string priorityQueueName)
        {
            if (string.IsNullOrEmpty(priorityQueueName) == false && !_priorityMessagesStatistics.ContainsKey(priorityQueueName))
            {
                _priorityMessagesStatistics[priorityQueueName] = new MessageStatistics();
            }
        }

        public void PriorityMessageHandeled(string priorityQueueName, MessageHandleStatus messageHandleStatus, double duration)
        {
            InitiatePriorityMessagesStatistics(priorityQueueName);

            lock(_priorityMessagesStatistics[priorityQueueName])
            {
                _priorityMessagesStatistics[priorityQueueName].MessageHandled(messageHandleStatus, duration);
            }
        }

        public Dictionary<string, MessageStatistics> GetPriorityMessagesStatistics()
        {
            return _priorityMessagesStatistics;
        }


        public void ResetPriorityMessagesStatistics()
        {
            _priorityMessagesStatistics = new();
        }
    }
}
