namespace CMH.Common.Repository
{
    public interface IRuntimeStatisticsRepository
    {
        void PriorityQueueQueried(int messageFetched, int duration);
    }

    public class RuntimeStatisticsRepository : IRuntimeStatisticsRepository
    {
        public int PriorityQueueQueries { get; private set; }
        public int TotalMessagesFetched { get; private set; }
        public int TotalMessageFetchDuration { get; private set; }

        public decimal AvgMessagesPerQuery
        {
            get { return Math.Round((decimal)(TotalMessagesFetched / PriorityQueueQueries), 2); }
        }

        public decimal AvgMessagesFetchDuration
        {
            get { return Math.Round((decimal)(TotalMessageFetchDuration / TotalMessagesFetched), 2); }
        }

        public void PriorityQueueQueried(int messageFetched, int duration)
        {
            PriorityQueueQueries++;
            TotalMessagesFetched += messageFetched;
            TotalMessageFetchDuration += duration;
        }
    }
}
