namespace CMH.Data.Model
{
    public class RuntimeStatistics
    {
        public int PriorityQueueQueries { get; set; }
        public int TotalMessagesFetched { get; set; }
        public int TotalMessagesProcessed { get; set; }
        public double TotalMessageFetchDuration { get; set; }
        public double TotalProcessDuration { get; set; }
        public long TotalMemoryUsage { get; set; }

        public double AvgMessagesPerQuery
        {
            get { return Math.Round((double)(TotalMessagesFetched / PriorityQueueQueries), 2); }
        }

        public double AvgMessagesFetchDuration
        {
            get { return Math.Round((double)(TotalMessageFetchDuration / TotalMessagesFetched), 2); }
        }

        //$0.000016 for every GB-s of memory (first 400 000 GB-s is free)
        public double ApproxExecutionTimeCost
        {
            get { return ((TotalMemoryUsage / (TotalProcessDuration / 1000)) * TotalMessagesProcessed) * 0.000016; }
        }

        //$0.2 for every 1 000 000 execution (first 1 000 000 is free)
        public double ApproxTotalExecutionCost
        {
            get { return (TotalMessagesProcessed / 1000000) * 0.20; }
        }
    }
}
