using Azure.Messaging.ServiceBus;

namespace CMH.Common.Repository
{
    public class MessageStatistics
    {
        public ServiceBusReceivedMessage? Message { get; set; }

        public DateTimeOffset HandledTime { get; set; }
    }
}
