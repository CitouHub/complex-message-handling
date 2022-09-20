using CMH.Common.Variable;
using System.ComponentModel.DataAnnotations;

namespace CMH.Data.Model
{
    public class RuntimeStatistics
    {
        public int PriorityQueueQueries { get; set; }
        public int TotalMessagesFetched { get; set; }
        public int TotalMessagesProcessed { get; set; }
        public double TotalFetchDuration { get; set; }
        public double TotalProcessDuration { get; set; }
        public long TotalMemoryUsage { get; set; }
        public short MaxParallellTasks { get; set; }
        public short AvgParallellTasks { get; set; }
        public DateTimeOffset? SessionStart { get; set; }
        public DateTimeOffset? SessionStop { get; set; }

        public double AvgMessagesPerQuery
        {
            get { return PriorityQueueQueries > 0 ? Math.Round((double)TotalMessagesFetched / PriorityQueueQueries, 2) : 0; }
        }

        public double AvgMessagesFetchDuration
        {
            get { return TotalMessagesFetched > 0 ? Math.Round(TotalFetchDuration / TotalMessagesFetched, 2) : 0; }
        }

        public double AvgThroughPut
        {
            get {
                var duration = (SessionStop - SessionStart)?.TotalMilliseconds;
                return duration != null && duration > 0 ? Math.Round(TotalMessagesProcessed / ((double)duration / 1000), 2) : 0; 
            }
        }

        //$0.000016 for every GB-s of memory (first 400 000 GB-s is free)
        public double ApproxExecutionTimeCost
        {
            get { return TotalProcessDuration > 0 ? ((TotalMemoryUsage / Economy.Function.ByteToGigaByteFactor) / (TotalProcessDuration / 1000)) * Economy.Function.GBsCost : 0; }
        }

        //$0.2 for every 1 000 000 execution (first 1 000 000 is free)
        public double ApproxTotalExecutionCost
        {
            get { return (TotalMessagesProcessed / Economy.Function.ExecutionBatch) * Economy.Function.ExecustionBatchCost; }
        }
    }
}
