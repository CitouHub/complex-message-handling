using System;
using System.Collections.Generic;

namespace CMH.Process.Util
{
    public static class RuntimeTracker
    {
        public static int TotalMessagesProcessed { get; private set; }
        public static double TotalProcessDuration { get; private set; }
        public static long TotalMemoryUsage { get; private set; }
        public static DateTimeOffset? SessionStart { get; private set; }
        public static DateTimeOffset? SessionStop { get; private set; }
        public static List<short> ParallellTasks { get; private set; } = new();

        private static short _parallellTasks = 0;
        private static object _statLock = new();

        public static void StartSession()
        {
            SessionStart = SessionStart != null ? SessionStart : DateTimeOffset.UtcNow;
            lock (_statLock)
            {
                _parallellTasks++;
                ParallellTasks.Add(_parallellTasks);
            }
        }

        public static void StopSession(double duration)
        {
            SessionStop = DateTimeOffset.UtcNow;
            _parallellTasks--;
            var currentProc = System.Diagnostics.Process.GetCurrentProcess();
            var memoryUsed = currentProc.PrivateMemorySize64;
            TotalProcessDuration += duration;
            TotalMemoryUsage += memoryUsed;
            TotalMessagesProcessed++;
        }

        public static void Reset()
        {
            TotalMessagesProcessed = 0;
            TotalProcessDuration = 0;
            TotalMemoryUsage = 0;
            SessionStart = null;
            SessionStop = null;
            ParallellTasks = new();
        }
    }
}
