using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IRuntimeStatisticsRepository
    {
        void PriorityQueueQueried(int messageFetched, double duration);
        RuntimeStatistics GetRuntimeStatistics();
        void ResetRuntimeStatistics();
    }

    public class RuntimeStatisticsRepository : IRuntimeStatisticsRepository
    {
        private RuntimeStatistics _runtimeStatistics = new();

        public void PriorityQueueQueried(int messageFetched, double duration)
        {
            _runtimeStatistics.PriorityQueueQueries++;
            _runtimeStatistics.TotalMessagesFetched += messageFetched;
            _runtimeStatistics.TotalFetchDuration += duration;
        }

        public RuntimeStatistics GetRuntimeStatistics()
        {
            return _runtimeStatistics;
        }

        public void ResetRuntimeStatistics()
        {
            _runtimeStatistics = new();
        }
    }
}
