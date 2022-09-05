using Azure.Messaging.ServiceBus;

namespace CMH.Data.Model
{
    public class MessageStatistics
    {
        public ServiceBusReceivedMessage? Message { get; set; }

        public DateTimeOffset HandledTime { get; set; }
    }
}
