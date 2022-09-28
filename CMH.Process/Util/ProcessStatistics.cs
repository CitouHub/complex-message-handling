using System.Collections.Generic;
using System.Linq;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Util
{
    public static class ProcessStatistics
    {
        public static readonly Dictionary<ProcessChannel, MessageStatistics> ProcessChannels = new ()
        {
            { ProcessChannel.Default, new MessageStatistics() },
            { ProcessChannel.Slow1, new MessageStatistics() },
            { ProcessChannel.Slow2, new MessageStatistics() },
            { ProcessChannel.Slow3, new MessageStatistics() }
        };

        public static void MessageHandeled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            lock(ProcessChannels[processChannel])
            {
                ProcessChannels[processChannel].MessageHandled(messageHandleStatus, duration);
            }
        }

        public static void Reset()
        {
            ProcessChannels.Keys.ToList().ForEach(_ => ProcessChannels[_] = new MessageStatistics());
        }
    }
}
