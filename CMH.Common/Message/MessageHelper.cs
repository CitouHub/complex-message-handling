using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace CMH.Common.Message
{
    public static class MessageHelper
    {
        public static ServiceBusMessage CreateMessage(short? dataSourceId)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var jobMessage = new JobMessage()
            {
                JobId = random.Next(100000),
                DataSourceId = dataSourceId ?? -1,
                JobName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                Description = "This is a seeded test message"
            };
            var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(jobMessage));
            serviceBusMessage.ApplicationProperties["DataSourceId"] = jobMessage.DataSourceId;
            serviceBusMessage.ApplicationProperties["Tries"] = 0;
            serviceBusMessage.ApplicationProperties["EnqueuedTime"] = DateTimeOffset.UtcNow;

            return serviceBusMessage;
        }
    }
}
