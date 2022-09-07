namespace CMH.Priority.Infrastructure
{
    public interface IQueueCache
    {
        void SetQueueList(List<string> queueList);
        List<string> GetQueueList();
        Task AwaitReadyAsync(CancellationToken cancellationToken);
    }

    public class QueueCache : IQueueCache
    {
        private readonly SemaphoreSlim _readySignal;
        private readonly object _lock = new();

        private List<string> _queueList = new();
        private short _waiting;

        public QueueCache()
        {
            _readySignal = new SemaphoreSlim(0);
        }

        public void SetQueueList(List<string> queueList)
        {
            lock(_lock)
            {
                _queueList = queueList.OrderBy(_ => _).ToList();
                if(_waiting > 0)
                {
                    _readySignal.Release(_waiting);
                }
            }
        }

        public List<string> GetQueueList()
        {
            lock (_lock)
            {
                return _queueList;
            }
        }

        public async Task AwaitReadyAsync(CancellationToken cancellationToken)
        {
            while (_queueList.Count == 0)
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
