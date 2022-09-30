using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using CMH.Process.Service;

namespace CMH.Process
{
    public class StatisticsFunction
    {
        private readonly IProcessStatisticsService _processStatisticsService;

        public StatisticsFunction(IProcessStatisticsService processStatisticsService)
        {
            _processStatisticsService = processStatisticsService;
        }

        [FunctionName("PostStatistics")]
        public void Run([TimerTrigger("*/2 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            _processStatisticsService.FlushPendingHandeledProcessMessages();
        }
    }
}
