using Azure.Messaging.ServiceBus;

namespace CMH.Common.Repository
{
    public interface IMessageStatisticsRepository
    {
        void PriorityMessageHandled(ServiceBusReceivedMessage message, short priority);
        void PriorityMessageRescheduled(int rescheduledTime);
        void ProcessMessageHandled(ServiceBusReceivedMessage message, string processChannel);
        void ProcessMessageRescheduled(int rescheduledTime);
    }

    public class MessageStatisticsRepository : IMessageStatisticsRepository
    {
        private const int RecentMessagesSize = 1000;

        public Dictionary<short, List<MessageStatistics>> RecentPriorityMessageHandled { get; private set; } = new();
        public int TotalPriorityMessagesHandled { get; private set; }
        public int TotalPriorityMessagesRescheduled { get; private set; }
        public double TotalTimeInPriorityQueue { get; private set; }
        public double TotalTimeInPriorityQueueRescheduled { get; private set; }

        public Dictionary<string, List<MessageStatistics>> RecentProcessMessagesHandled { get; private set; } = new();
        public int TotalProcessMessages { get; private set; }
        public int TotalProcessMessagesRescheduled { get; private set; }
        public double TotalTimeInProcessChannel { get; private set; }
        public double TotalTimeInProcessChannelRescheduled { get; private set; }

        public void PriorityMessageHandled(ServiceBusReceivedMessage message, short priority)
        {
            lock (RecentPriorityMessageHandled)
            {
                var handledTime = DateTimeOffset.UtcNow;
                if (!RecentPriorityMessageHandled.ContainsKey(priority))
                {
                    RecentPriorityMessageHandled[priority] = new List<MessageStatistics>();
                }

                RecentPriorityMessageHandled[priority].Add(new MessageStatistics() { Message = message, HandledTime = handledTime });
                if (RecentPriorityMessageHandled.Count > RecentMessagesSize)
                {
                    var lastMessage = RecentPriorityMessageHandled[priority].LastOrDefault();
                    if (lastMessage != null)
                    {
                        RecentPriorityMessageHandled[priority].Remove(lastMessage);
                    }
                }

                TotalPriorityMessagesHandled++;
                TotalTimeInPriorityQueue += (handledTime - message.EnqueuedTime).TotalMilliseconds;
            }
        }

        public void PriorityMessageRescheduled(int rescheduledTime)
        {
            TotalPriorityMessagesRescheduled++;
            TotalTimeInPriorityQueueRescheduled += rescheduledTime;
        }

        public void ProcessMessageHandled(ServiceBusReceivedMessage message, string processChannel)
        {
            lock (RecentProcessMessagesHandled)
            {
                var handledTime = DateTimeOffset.UtcNow;
                if (!RecentProcessMessagesHandled.ContainsKey(processChannel))
                {
                    RecentProcessMessagesHandled[processChannel] = new List<MessageStatistics>();
                }

                RecentProcessMessagesHandled[processChannel].Add(new MessageStatistics() { Message = message, HandledTime = handledTime });
                if (RecentProcessMessagesHandled.Count > RecentMessagesSize)
                {
                    var lastMessage = RecentProcessMessagesHandled[processChannel].LastOrDefault();
                    if (lastMessage != null)
                    {
                        RecentProcessMessagesHandled[processChannel].Remove(lastMessage);
                    }
                }

                TotalProcessMessages++;
                TotalTimeInProcessChannel += (handledTime - message.EnqueuedTime).TotalMilliseconds;
            }
        }

        public void ProcessMessageRescheduled(int rescheduledTime)
        {
            TotalProcessMessagesRescheduled++;
            TotalTimeInProcessChannelRescheduled += rescheduledTime;
        }
    }
}
