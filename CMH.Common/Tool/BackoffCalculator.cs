namespace CMH.Common.Tool
{
    public static class BackoffCalculator
    {
        public static int CalculateIterationSleepTime(int initialSleepTime, double backoffFactor, int iterations, int maxSleepTime)
        {
            var sleepTime = (int)Math.Round(Math.Pow(backoffFactor, iterations - 1) * initialSleepTime, 0);
            return sleepTime <= maxSleepTime ? sleepTime : maxSleepTime;
        }

        public static int CalculatePriorityRescheduleSleepTime(int initialSleepTime, double tryFactor, double priorityFactor, short priority, int tries, int maxSleepTime)
        {
            var sleepTime = (int)Math.Round(initialSleepTime * tryFactor * tries * priorityFactor * priority, 0);
            return sleepTime <= maxSleepTime ? sleepTime : maxSleepTime;
        }

        public static int CalculateProcessRescheduleSleepTime(int initialSleepTime, double backOffFactor, int tries)
        {
            return (int)Math.Round(initialSleepTime * Math.Pow(backOffFactor, tries));
        }
    }
}
