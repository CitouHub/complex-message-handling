using System.Diagnostics;

namespace CMH.Data.Repository
{
    public interface IRuntimeStatisticsRepository
    {
        void PriorityQueueQueried(int messageFetched, double duration);
        void MessageProcessingFinished(double duration);
    }

    public class RuntimeStatisticsRepository : IRuntimeStatisticsRepository
    {
        public int PriorityQueueQueries { get; private set; }
        public int TotalMessagesFetched { get; private set; }
        public double TotalMessageFetchDuration { get; private set; }
        public double TotalProcessDuration { get; private set; }
        public long TotalMemoryUsage { get; private set; }

        public double AvgMessagesPerQuery
        {
            get { return Math.Round((double)(TotalMessagesFetched / PriorityQueueQueries), 2); }
        }

        public double AvgMessagesFetchDuration
        {
            get { return Math.Round((double)(TotalMessageFetchDuration / TotalMessagesFetched), 2); }
        }

        public void PriorityQueueQueried(int messageFetched, double duration)
        {
            PriorityQueueQueries++;
            TotalMessagesFetched += messageFetched;
            TotalMessageFetchDuration += duration;
        }

        public void MessageProcessingFinished(double duration)
        {
            var currentProc = Process.GetCurrentProcess();
            var memoryUsed = currentProc.PrivateMemorySize64;
            TotalProcessDuration += duration;
            TotalMemoryUsage += memoryUsed;
        }
    }
}
