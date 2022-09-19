using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

using CMH.Common.Message;
using CMH.Priority.Infrastructure;

namespace CMH.Priority.Service
{
    public class MessageSeederService : BackgroundService
    {
        private readonly ILogger<MessageSeederService> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IQueueCache _queueCache;

        public MessageSeederService(
            ILogger<MessageSeederService> logger,
            ServiceBusClient serviceBusClient,
            IQueueCache queueCache)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            _queueCache = queueCache;

            _logger.LogDebug("Instantiated");
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _queueCache.AwaitReadyAsync(cancellationToken);

            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            foreach (var priorityQueue in _queueCache.GetPriorityQueues())
            {
                var messages = new List<ServiceBusMessage>();

                for(var i = 0; i<500; i++)
                {
                    var jobMessage = new JobMessage()
                    {
                        JobId = random.Next(100000),
                        DataSourceId = (short)random.Next(1, 10),
                        JobName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        Description = "This is a seeded test message"
                    };
                    var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(jobMessage));
                    serviceBusMessage.ApplicationProperties["DataSourceId"] = jobMessage.DataSourceId;
                    serviceBusMessage.ApplicationProperties["Tries"] = 0;
                    serviceBusMessage.ApplicationProperties["EnqueuedTime"] = DateTimeOffset.UtcNow;

                    messages.Add(serviceBusMessage);
                }

                var sender = _serviceBusClient.CreateSender(priorityQueue.Name);
                await sender.SendMessagesAsync(messages, cancellationToken);
            }
        }
    }
}
