using System.Diagnostics;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IRuntimeStatisticsRepository
    {
        void PriorityQueueQueried(int messageFetched, double duration);
        void MessageProcessingFinished(double duration);
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
            _runtimeStatistics.TotalMessageFetchDuration += duration;
        }

        public void MessageProcessingFinished(double duration)
        {
            var currentProc = Process.GetCurrentProcess();
            var memoryUsed = currentProc.PrivateMemorySize64;
            _runtimeStatistics.TotalProcessDuration += duration;
            _runtimeStatistics.TotalMemoryUsage += memoryUsed;
            _runtimeStatistics.TotalMessagesProcessed++;
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
