using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using CMH.Process.Service;
using Azure.Messaging.ServiceBus;

namespace CMH.Process
{
    public class StatisticsFunction
    {
        private readonly IProcessStatisticsService _processStatisticsService;

        public StatisticsFunction(IProcessStatisticsService processStatisticsService)
        {
            _processStatisticsService = processStatisticsService;
        }

        [FunctionName("GetStatistics")]
        public IActionResult GetStatistics([HttpTrigger(AuthorizationLevel.Function, "get", Route = "statistics")] HttpRequest req, ILogger log)
        {
            return new OkObjectResult(_processStatisticsService.GetProcessChannels());
        }

        [FunctionName("ResetStatistics")]
        public IActionResult ResetStatistics([HttpTrigger(AuthorizationLevel.Function, "post", Route = "statistics/reset")] HttpRequest req, ILogger log)
        {
            _processStatisticsService.Reset();
            return new OkResult();
        }
    }
}
