using Azure.Messaging.ServiceBus.Administration;
using CMH.Priority.Infrastructure;
using CMH.Priority.Util;

namespace CMH.Priority.Service
{
    public class QueueCacheService : BackgroundService
    {
        private readonly Config _config;
        private readonly ILogger<QueueCacheService> _logger;
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
        private readonly IQueueCache _queueCache;

        private Timer? _timer;

        public QueueCacheService(
            Config config,
            ILogger<QueueCacheService> logger,
            ServiceBusAdministrationClient serviceBusAdministrationClient,
            IQueueCache queueCache)
        {
            _config = config;
            _logger = logger;
            _serviceBusAdministrationClient = serviceBusAdministrationClient;
            _queueCache = queueCache;

            _logger.LogDebug("Instantiated");
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _timer = new Timer(async _ => await UpdateQueueCache(cancellationToken), null, 0, Timeout.Infinite);
            }

            return Task.CompletedTask;
        }

        private async Task UpdateQueueCache(CancellationToken cancellationToken)
        {
            var queueNames = new List<string>();

            var queues = _serviceBusAdministrationClient.GetQueuesAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await queues.MoveNextAsync())
            {
                var queue = queues.Current;
                if (queue.Name.ToLower().StartsWith("priority") &&
                    queue.Status != EntityStatus.Disabled &&
                    queue.Status != EntityStatus.ReceiveDisabled)
                {
                    queueNames.Add(queue.Name);
                }
            }

            _queueCache.SetQueueList(queueNames);

            if (_timer != null)
            {
                var nextRefresh = _config.QueueCache.RefreshInterval * 1000;
                _timer.Change(nextRefresh, Timeout.Infinite);
            }
        }
    }
}
