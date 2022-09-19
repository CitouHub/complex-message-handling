namespace CMH.Common.Util
{
    public static class BackoffCalculator
    {
        public static int CalculateIterationSleepTime(int initialSleepTime, double backoffFactor, int iterations, int maxSleepTime)
        {
            var sleepTime = (int)Math.Round(initialSleepTime * Math.Pow(backoffFactor, Math.Max(iterations - 1, 0)), 0);
            return HandleNegative(sleepTime <= maxSleepTime ? sleepTime : maxSleepTime);
        }

        public static int CalculatePriorityRescheduleSleepTime(int initialSleepTime, double tryFactor, double priorityFactor, int tries, short priorityIndex, int maxSleepTime)
        {
            var sleepTime = (int)Math.Round(initialSleepTime * tryFactor * tries * priorityFactor * (priorityIndex + 1), 0);
            return HandleNegative(sleepTime <= maxSleepTime ? sleepTime : maxSleepTime);
        }

        public static int CalculateProcessRescheduleSleepTime(int initialSleepTime, double backOffFactor, int tries)
        {
            return HandleNegative((int)Math.Round(initialSleepTime * Math.Pow(backOffFactor, Math.Max(tries, 0))));
        }

        private static int HandleNegative(int value)
        {
            return value < 0 ? 0 : value;
        }
    }
}
