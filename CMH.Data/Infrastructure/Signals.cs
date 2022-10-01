namespace CMH.Data.Infrastructure
{
    public static class Signals
    {
        public static object MessageStatisticsRepository_PriorityLock = new();
        public static object MessageStatisticsRepository_ProcessLock = new();
    }
}
