namespace CMH.Data.Model
{
    public class RuntimeStatistics
    {
        public int PriorityQueueQueries { get; set; }
        public int TotalMessagesFetched { get; set; }
        public double TotalFetchDuration { get; set; }

        public double AvgMessagesPerQuery
        {
            get { return PriorityQueueQueries > 0 ? Math.Round((double)TotalMessagesFetched / PriorityQueueQueries, 2) : 0; }
        }

        public double AvgMessagesFetchDuration
        {
            get { return TotalMessagesFetched > 0 ? Math.Round(TotalFetchDuration / TotalMessagesFetched, 2) : 0; }
        }
    }
}
