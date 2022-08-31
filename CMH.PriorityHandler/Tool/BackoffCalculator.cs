namespace CMH.PriorityHandler.Tool
{
    public static class BackoffCalculator
    {
        public static int CalculateRescheduleSleepTime(Config config, short priority, int tries)
        {
            var sleepTime = (int)Math.Round(
                config.Process.BackoffPolicy.InitialSleepTime *
                config.Process.BackoffPolicy.TrySleepTimeFactor * tries *
                config.Process.BackoffPolicy.PrioritySleepTimeFactor * priority, 0);
            return sleepTime <= config.Process.BackoffPolicy.MaxSleepTime ? sleepTime : config.Process.BackoffPolicy.MaxSleepTime;
        }

        public static int CalculateIterationSleepTime(Config config, int iterations)
        {
            var sleepTime = (int)Math.Round(Math.Pow(config.Priority.BackoffPolicy.BackoffSleepTimeFactor, iterations - 1) *
                config.Priority.BackoffPolicy.InitialSleepTime, 0);
            return sleepTime <= config.Priority.BackoffPolicy.MaxSleepTime ? sleepTime : config.Priority.BackoffPolicy.MaxSleepTime;
        }
    }
}
