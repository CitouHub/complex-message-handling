using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageCompleted(string priorityQueueName, double duration);
        void PriorityMessageRescheduled(string priorityQueueName);
        void ProcessMessageCompleted(ProcessChannel processChannel, double duration);
        void ProcessMessageRescheduled(ProcessChannel processChannel);
        void ProcessMessageDiscarded(ProcessChannel processChannel);
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
            if (!_priorityMessagesStatistics.ContainsKey(priorityQueueName))
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

        public void PriorityMessageCompleted(string priorityQueueName, double duration)
        {
            lock (_priorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priorityQueueName);

                _priorityMessagesStatistics[priorityQueueName].TotalMessagesHandled++;
                _priorityMessagesStatistics[priorityQueueName].TotalMessageDuration += duration;
            }
        }

        public void PriorityMessageRescheduled(string priorityQueueName)
        {
            lock (_priorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priorityQueueName);

                _priorityMessagesStatistics[priorityQueueName].TotalMessagesRescheduled++;
            }
        }

        public void ProcessMessageCompleted(ProcessChannel processChannel, double duration)
        {
            lock (_processMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                _processMessagesStatistics[processChannel].TotalMessagesHandled++;
                _processMessagesStatistics[processChannel].TotalMessageDuration += duration;
            }
        }

        public void ProcessMessageRescheduled(ProcessChannel processChannel)
        {
            lock (_processMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                _processMessagesStatistics[processChannel].TotalMessagesRescheduled++;
            }
        }

        public void ProcessMessageDiscarded(ProcessChannel processChannel)
        {
            lock (_processMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                _processMessagesStatistics[processChannel].TotalMessagesDiscarded++;
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
