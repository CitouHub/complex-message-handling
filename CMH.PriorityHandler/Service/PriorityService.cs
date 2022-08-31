using System.Text.RegularExpressions;

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

using CMH.Common.Extenstion;
using CMH.PriorityHandler.Infrastructure;
using CMH.PriorityHandler.Tool;

namespace CMH.PriorityHandler.Service
{
    public class PriorityService : BackgroundService
    {
        private readonly Config _config;
        private readonly ILogger<PriorityService> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
        private readonly IQueueCache _queueCache;

        private int iterations;
        private Timer? _timer;

        public PriorityService(
            Config config,
            ILogger<PriorityService> logger,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient,
            IQueueCache queueCache)
        {
            _logger = logger;
            _config = config;
            _serviceBusClient = serviceBusClient;
            _serviceBusAdministrationClient = serviceBusAdministrationClient;
            _queueCache = queueCache;

            iterations = 0;

            _logger.LogDebug("Instantiated");
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _queueCache.AwaitReadyAsync(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                _timer = new Timer(async _ => await HandleMessages(cancellationToken), null, 0, Timeout.Infinite);
            }
        }

        private async Task HandleMessages(CancellationToken cancellationToken)
        {
            var queueIndex = 0;
            var messageBatch = _config.Priority.MessageBatch;

            while (queueIndex < _queueCache.GetQueueList().Count && 
                cancellationToken.IsCancellationRequested == false)
            {
                var queueName = _queueCache.GetQueueList()[queueIndex];
                var priority = (short)(short.Parse(Regex.Replace(queueName, "[^0-9.]", "")) / 10);
                var receiver = _serviceBusClient.CreateReceiver(queueName);
                
                var priorityMessages = await receiver.ReceiveMessagesAsync(messageBatch, TimeSpan.FromMilliseconds(1000), cancellationToken);

                if (priorityMessages != null && priorityMessages.Count > 0)
                {
                    queueIndex = 0;
                    iterations = 0;
                    foreach (var dataSourceMessages in priorityMessages.GroupBy(_ => new { DataSourceId = (short)_.ApplicationProperties["DataSourceId"] }))
                    {
                        var processChannel = _config.Process.DataSourceProcessChannelMap.ContainsKey(dataSourceMessages?.Key.DataSourceId ?? -1) == true ?
                             _config.Process.DataSourceProcessChannelMap[dataSourceMessages?.Key.DataSourceId ?? -1] : _config.Process.DefaultProcessChannel;

                        var sender = _serviceBusClient.CreateSender(processChannel);
                        var queueProperties = await _serviceBusAdministrationClient.GetQueueRuntimePropertiesAsync(processChannel, cancellationToken);

                        var messageCount = (int)queueProperties.Value.TotalMessageCount;
                        var availibleSpots = _config.Process.BackoffPolicy.ProcessChannelSize - messageCount;
                        var prioritySpotReduction = priority * _config.Process.BackoffPolicy.PriorityStepSize;
                        var availiblePrioritySpots = availibleSpots - prioritySpotReduction;

                        var messagesToProcess = new List<ServiceBusReceivedMessage>();
                        var messagesToReschedule = new List<ServiceBusReceivedMessage>();
                        dataSourceMessages?.ToList().Split(availiblePrioritySpots, out messagesToProcess, out messagesToReschedule);

                        messagesToProcess.ForEach(async _ => await receiver.CompleteMessageAsync(_));
                        messagesToReschedule.ForEach(async _ => await receiver.CompleteMessageAsync(_));

                        var messages = messagesToProcess.Select(_ => new ServiceBusMessage(_)).ToList();
                        messages.ForEach(_ => _.ApplicationProperties.Clear());
                        await sender.SendMessagesAsync(messages, cancellationToken);

                        if (messagesToReschedule.Count > 0)
                        {
                            var returnSender = _serviceBusClient.CreateSender(queueName);
                            messagesToReschedule.ForEach(async _ => await returnSender.RescheduleMessageAsync(_,
                                DateTimeOffset.UtcNow.AddSeconds(BackoffCalculator.CalculateRescheduleSleepTime(_config, priority, (int)_.ApplicationProperties["Tries"]))));
                        }
                    }
                }
                else
                {
                    queueIndex++;
                }
            }

            iterations++;
            if (_timer != null)
            {
                var nextInteration = BackoffCalculator.CalculateIterationSleepTime(_config, iterations);
                _timer.Change(nextInteration * 1000, Timeout.Infinite);
            }
        }
    }
}