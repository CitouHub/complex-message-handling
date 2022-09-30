using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Service
{
    public interface IProcessStatisticsService
    {
        Dictionary<ProcessChannel, MessageStatistics> GetProcessChannels();
        void MessageHandeled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration);
        void Reset();
    }

    public class ProcessStatisticsService : IProcessStatisticsService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string CacheKey = "ProcessChannels";

        public ProcessStatisticsService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Dictionary<ProcessChannel, MessageStatistics> GetProcessChannels()
        {
            var processChannels = _memoryCache.Get<Dictionary<ProcessChannel, MessageStatistics>>(CacheKey);
            return processChannels ?? new Dictionary<ProcessChannel, MessageStatistics>();
        }

        public void MessageHandeled(ProcessChannel processChannel, MessageHandleStatus messageHandleStatus, double duration)
        {
            lock(_memoryCache)
            {
                var processChannels = _memoryCache.Get<Dictionary<ProcessChannel, MessageStatistics>>(CacheKey);
                processChannels ??= new Dictionary<ProcessChannel, MessageStatistics>();

                if (processChannels.ContainsKey(processChannel) == false)
                {
                    processChannels.Add(processChannel, new MessageStatistics());
                }

                processChannels[processChannel].MessageHandled(messageHandleStatus, duration);
                _memoryCache.Set(CacheKey, processChannels);
            }
        }

        public void Reset()
        {
            lock (_memoryCache)
            {
                _memoryCache.Set(CacheKey, new Dictionary<ProcessChannel, MessageStatistics>());
            }
        }
    }
}
