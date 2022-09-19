using CMH.Data.Model;

namespace CMH.Priority.Infrastructure
{
    public interface IQueueCache
    {
        void SetPriorityQueues(List<PriorityQueue> priorityQueues);
        List<PriorityQueue> GetPriorityQueues();
        Task AwaitReadyAsync(CancellationToken cancellationToken);
    }

    public class QueueCache : IQueueCache
    {
        private readonly SemaphoreSlim _readySignal;
        private readonly object _lock = new();

        private List<PriorityQueue> _priorityQueues = new();
        private short _waiting;

        public QueueCache()
        {
            _readySignal = new SemaphoreSlim(0);
        }

        public void SetPriorityQueues(List<PriorityQueue> priorityQueues)
        {
            lock (_lock)
            {
                _priorityQueues = priorityQueues.OrderBy(_ => _.Name).ToList();
                if (_waiting > 0)
                {
                    _readySignal.Release(_waiting);
                }
            }
        }

        public List<PriorityQueue> GetPriorityQueues()
        {
            return _priorityQueues;
        }

        public async Task AwaitReadyAsync(CancellationToken cancellationToken)
        {
            while (_priorityQueues.Count == 0)
            {
                lock (_lock)
                {
                    _waiting++;
                }
                await _readySignal.WaitAsync(cancellationToken);
            }
        }
    }
}
