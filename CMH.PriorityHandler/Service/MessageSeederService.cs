using Azure.Messaging.ServiceBus;
using CMH.Common.Message;
using CMH.PriorityHandler.Infrastructure;
using Newtonsoft.Json;

namespace CMH.PriorityHandler.Service
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

            foreach (var queue in _queueCache.GetQueueList())
            {
                var messages = new List<ServiceBusMessage>();

                for(var i = 0; i<150; i++)
                {
                    var jobMessage = new JobMessage()
                    {
                        JobId = random.Next(100000),
                        DataSourceId = (short)random.Next(1, 10),
                        JobName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        Description = "This is a seeded test message"
                    };
                    var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(jobMessage));
                    serviceBusMessage.ApplicationProperties["Tries"] = 0;
                    serviceBusMessage.ApplicationProperties["DataSourceId"] = jobMessage.DataSourceId;

                    messages.Add(serviceBusMessage);
                }

                var sender = _serviceBusClient.CreateSender(queue);
                await sender.SendMessagesAsync(messages, cancellationToken);
            }
        }
    }
}
