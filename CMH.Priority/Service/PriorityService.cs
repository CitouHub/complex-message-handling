﻿using System.Text.RegularExpressions;

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

using CMH.Common.Extenstion;
using CMH.Common.Util;
using CMH.Common.Variable;
using CMH.Data.Repository;
using CMH.Priority.Infrastructure;
using CMH.Priority.Util;

namespace CMH.Priority.Service
{
    public class PriorityService : BackgroundService
    {
        private readonly Config _config;
        private readonly ILogger<PriorityService> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
        private readonly IQueueCache _queueCache;
        private readonly IMessageStatisticsRepository _messageStatisticsRepository;
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository;
        private readonly IDataSourceRepository _dataSourceRepository;

        private int iterations;
        private short activeTasks;

        public PriorityService(
            Config config,
            ILogger<PriorityService> logger,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient,
            IQueueCache queueCache,
            IMessageStatisticsRepository messageStatisticsRepository,
            IRuntimeStatisticsRepository runtimeStatisticsRepository,
            IDataSourceRepository dataSourceRepository)
        {
            _logger = logger;
            _config = config;
            _serviceBusClient = serviceBusClient;
            _serviceBusAdministrationClient = serviceBusAdministrationClient;
            _queueCache = queueCache;
            _messageStatisticsRepository = messageStatisticsRepository;
            _runtimeStatisticsRepository = runtimeStatisticsRepository;
            _dataSourceRepository = dataSourceRepository;

            iterations = 0;

            _logger.LogDebug("Instantiated");
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _queueCache.AwaitReadyAsync(cancellationToken);

            while(cancellationToken.IsCancellationRequested == false)
            {
                if (activeTasks < _config.Priority.Tasks)
                {
                    _ = Task.Run(() => HandleMessagesAsync(cancellationToken), cancellationToken);
                    activeTasks++;
                }
                Thread.Sleep(5000);
            }
        }

        private async Task HandleMessagesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"New HandleMessagesAsync task started ({activeTasks}/{_config.Priority.Tasks})");
            var queueIndex = 0;

            while (cancellationToken.IsCancellationRequested == false &&
                activeTasks <= _config.Priority.Tasks)
            {
                try
                {
                    while (cancellationToken.IsCancellationRequested == false &&
                        queueIndex < _queueCache.GetPriorityQueues().Count)
                    {
                        var priorityQueue = _queueCache.GetPriorityQueues()[queueIndex];
                        _logger.LogInformation($"Fetching new messages from {priorityQueue.Name}");

                        var receiver = _serviceBusClient.CreateReceiver(priorityQueue.Name);
                        var priorityMessages = await GetPriorityMessageAsync(receiver, priorityQueue.Name, cancellationToken);
                        _logger.LogInformation($"{(priorityMessages != null ? priorityMessages.Count : 0)} messages fetched");

                        if (cancellationToken.IsCancellationRequested == false && priorityMessages != null && priorityMessages.Count > 0)
                        {
                            queueIndex = 0;
                            iterations = 0;
                            foreach (var dataSourceMessages in priorityMessages.GroupBy(_ => new { DataSourceId = (short)_.ApplicationProperties["DataSourceId"] }))
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    break;
                                }

                                var processChannelQueueName = $"{Queue.ProcessQueuePrefix}{_dataSourceRepository.Get(dataSourceMessages?.Key.DataSourceId ?? -1)?.ProcessChannel}";
                                _logger.LogInformation($"Handling messages for {dataSourceMessages?.Key.DataSourceId}, target {processChannelQueueName}");

                                var queueProperties = await _serviceBusAdministrationClient.GetQueueRuntimePropertiesAsync(processChannelQueueName, cancellationToken);
                                var availableSpots = GetAvailableProcessChannelSpots(
                                    queueProperties.Value.ActiveMessageCount,
                                    (short)_queueCache.GetPriorityQueues().IndexOf(priorityQueue), 
                                    (short)_queueCache.GetPriorityQueues().Count,
                                    _config.BackoffPolicy.ProcessChannelFull.PrioritySlots);
                                _logger.LogInformation($"{processChannelQueueName} has {availableSpots}");

                                var messagesToProcess = new List<ServiceBusReceivedMessage>();
                                var messagesToReschedule = new List<ServiceBusReceivedMessage>();
                                dataSourceMessages?.ToList().Split(availableSpots, out messagesToProcess, out messagesToReschedule);
                                messagesToProcess.ForEach(async _ => await receiver.CompleteMessageAsync(_));
                                messagesToReschedule.ForEach(async _ => await receiver.CompleteMessageAsync(_));
                                _logger.LogInformation($"{messagesToProcess.Count} messages will be forwarded for processing, {messagesToReschedule.Count} messages will be rescheduled");

                                var messages = messagesToProcess.Select(_ => new ServiceBusMessage(_)).ToList();
                                messages.ForEach(_ =>
                                {
                                    _.ApplicationProperties["Tries"] = 0;
                                    _.ApplicationProperties["EnqueuedTime"] = DateTimeOffset.UtcNow;
                                });

                                var sender = _serviceBusClient.CreateSender(processChannelQueueName);
                                await sender.SendMessagesAsync(messages, cancellationToken);
                                messagesToProcess.ForEach(_ => _messageStatisticsRepository.PriorityMessageCompleted(priorityQueue.Name, 
                                    (DateTimeOffset.UtcNow - (DateTimeOffset)_.ApplicationProperties["EnqueuedTime"]).TotalMilliseconds));
                                _logger.LogInformation($"Messages forwarded");

                                if (messagesToReschedule.Count > 0)
                                {
                                    var returnSender = _serviceBusClient.CreateSender(priorityQueue.Name);
                                    messagesToReschedule.ForEach(async _ => {
                                        var rescheduleTime = BackoffCalculator.CalculatePriorityRescheduleSleepTime(
                                            _config.BackoffPolicy.ProcessChannelFull.InitialSleepTime,
                                            _config.BackoffPolicy.ProcessChannelFull.TryFactor,
                                            _config.BackoffPolicy.ProcessChannelFull.PriorityFactor,
                                            (int)_.ApplicationProperties["Tries"],
                                            (short)_queueCache.GetPriorityQueues().IndexOf(priorityQueue),
                                            _config.BackoffPolicy.ProcessChannelFull.MaxSleepTime);
                                        await returnSender.RescheduleMessageAsync(_, DateTimeOffset.UtcNow.AddSeconds(rescheduleTime));
                                        _messageStatisticsRepository.PriorityMessageRescheduled(priorityQueue.Name);
                                    });
                                    _logger.LogInformation($"Messages reschduled");
                                }
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"No messages found in {priorityQueue.Name}, move to next queue");
                            queueIndex++;
                        }
                    }

                    iterations++;
                    var sleepTime = BackoffCalculator.CalculateIterationSleepTime(
                        _config.BackoffPolicy.EmptyIteration.InitialSleepTime,
                        _config.BackoffPolicy.EmptyIteration.BackoffFactor,
                        iterations,
                        _config.BackoffPolicy.EmptyIteration.MaxSleepTime);
                    await Task.Delay(sleepTime, cancellationToken);
                    queueIndex = 0;
                } 
                catch (Exception e)
                {
                    _logger.LogError($"Unexpected exception {e.Message}", e);
                }
            }

            activeTasks--;
            _logger.LogInformation($"HandleMessagesAsync task finished ({activeTasks}/{_config.Priority.Tasks})");
        }

        private async Task<IReadOnlyList<ServiceBusReceivedMessage>?> GetPriorityMessageAsync(ServiceBusReceiver receiver, string queueName, CancellationToken cancellationToken)
        {
            var messageBatch = _config.Priority.MessageBatch;
            var queueProperties = await _serviceBusAdministrationClient.GetQueueRuntimePropertiesAsync(queueName, cancellationToken);
            var queryTime = DateTimeOffset.UtcNow;
            if(queueProperties.Value.ActiveMessageCount > 0)
            {
                messageBatch = (short)(queueProperties.Value.ActiveMessageCount < messageBatch ? queueProperties.Value.ActiveMessageCount : messageBatch);
                var messages = await receiver.ReceiveMessagesAsync(messageBatch, TimeSpan.FromMilliseconds(messageBatch * _config.Priority.MessageFetchTimeOut), cancellationToken);
                _runtimeStatisticsRepository.PriorityQueueQueried(messages.Count, (DateTimeOffset.UtcNow - queryTime).TotalMilliseconds);
                return messages;
            }

            return null;
        }

        public int GetAvailableProcessChannelSpots(long messageCount, short priorityIndex, short totalPriorities, short prioritySlots) {

            var maxSize = totalPriorities * prioritySlots;
            var priorityGroupBlockedSize = priorityIndex * prioritySlots;
            var priorityGroupMaxSize = maxSize - priorityGroupBlockedSize;
            var availibleSpots = priorityGroupMaxSize - messageCount;
            return (int)(availibleSpots >= 0 ? availibleSpots : 0);
        }
    }
}