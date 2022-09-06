using Azure.Messaging.ServiceBus;

using CMH.Common.Enum;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageHandled(short priority, ServiceBusReceivedMessage message);
        void PriorityMessageRescheduled(short priority);
        void ProcessMessageHandled(ProcessChannel processChannel, ServiceBusReceivedMessage message);
        void ProcessMessageRescheduled(ProcessChannel processChannel);
        void ProcessMessageDiscarded(ProcessChannel processChannel);
    }

    public class MessageStatisticsRepository : IMessageStatisticsRepository
    {
        public Dictionary<short, MessageStatistics> PriorityMessagesStatistics { get; private set; } = new();
        public Dictionary<ProcessChannel, MessageStatistics> ProcessMessagesStatistics { get; private set; } = new();

        private void InitiatePriorityMessagesStatistics(short priority)
        {
            if (!PriorityMessagesStatistics.ContainsKey(priority))
            {
                PriorityMessagesStatistics[priority] = new MessageStatistics();
            }
        }

        private void InitiateProcessMessagesStatistics(ProcessChannel processChannel)
        {
            if (!ProcessMessagesStatistics.ContainsKey(processChannel))
            {
                ProcessMessagesStatistics[processChannel] = new MessageStatistics();
            }
        }

        public void PriorityMessageHandled(short priority, ServiceBusReceivedMessage message)
        {
            lock (PriorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priority);

                PriorityMessagesStatistics[priority].TotalMessagesHandled++;
                PriorityMessagesStatistics[priority].TotalMessageDuration =
                    (DateTimeOffset.UtcNow - (DateTimeOffset)message.ApplicationProperties["EnqueuedTime"]).TotalMilliseconds;
            }
        }

        public void PriorityMessageRescheduled(short priority)
        {
            lock (PriorityMessagesStatistics)
            {
                InitiatePriorityMessagesStatistics(priority);

                PriorityMessagesStatistics[priority].TotalMessagesRescheduled++;
            }
        }

        public void ProcessMessageHandled(ProcessChannel processChannel, ServiceBusReceivedMessage message)
        {
            lock (ProcessMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                ProcessMessagesStatistics[processChannel].TotalMessagesHandled++;
                ProcessMessagesStatistics[processChannel].TotalMessageDuration = 
                    (DateTimeOffset.UtcNow - (DateTimeOffset)message.ApplicationProperties["EnqueuedTime"]).TotalMilliseconds;
            }
        }

        public void ProcessMessageRescheduled(ProcessChannel processChannel)
        {
            lock (ProcessMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                ProcessMessagesStatistics[processChannel].TotalMessagesRescheduled++;
            }
        }

        public void ProcessMessageDiscarded(ProcessChannel processChannel)
        {
            lock (ProcessMessagesStatistics)
            {
                InitiateProcessMessagesStatistics(processChannel);

                ProcessMessagesStatistics[processChannel].TotalMessagesDiscarded++;
            }
        }
    }
}
