using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageCompleted(short priority, double duration);
        void PriorityMessageRescheduled(short priority);
        void ProcessMessageCompleted(ProcessChannel processChannel, double duration);
        void ProcessMessageRescheduled(ProcessChannel processChannel);
        void ProcessMessageDiscarded(ProcessChannel processChannel);
        Dictionary<short, MessageStatistics> GetPriorityMessagesStatistics();
        Dictionary<ProcessChannel, MessageStatistics> GetProcessMessagesStatistics();
        void ResetPriorityMessagesStatistics();
        void ResetProcessMessagesStatistics();
    }

    public class MessageStatisticsRepository : IMessageStatisticsRepository
    {
        private Dictionary<short, MessageStatistics> _priorityMessagesStatistics = new();
        private Dictionary<ProcessChannel, MessageStatistics> _processMessagesStatistics = new();

        private void InitiatePriorityMessagesStatistics(short priority)
        {
            if (!_priorityMessagesStatistics.ContainsKey(priority))
            {
                _priorityMessagesStatistics[priority] = new MessageStatistics();
            }
        }

        private void InitiateProcessMessagesStatistics(ProcessChannel processChannel)
        {
            if (!_processMessagesStatistics.ContainsKey(processChannel))
            {
                _processMessagesStatistics[processChannel] = new MessageStatistics();
            }
        }

        public void PriorityMessageCompleted(short priority, double duration)
        {
            lock (_priorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priority);

                _priorityMessagesStatistics[priority].TotalMessagesHandled++;
                _priorityMessagesStatistics[priority].TotalMessageDuration += duration;
            }
        }

        public void PriorityMessageRescheduled(short priority)
        {
            lock (_priorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priority);

                _priorityMessagesStatistics[priority].TotalMessagesRescheduled++;
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

        public Dictionary<short, MessageStatistics> GetPriorityMessagesStatistics()
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
