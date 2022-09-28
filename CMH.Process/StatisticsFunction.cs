using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using CMH.Process.Util;

namespace CMH.Process
{
    public class StatisticsFunction
    {
        [FunctionName("GetStatistics")]
        public static IActionResult GetStatistics([HttpTrigger(AuthorizationLevel.Function, "get", Route = "statistics")] HttpRequest req, ILogger log)
        {
            return new OkObjectResult(ProcessStatistics.ProcessChannels);
        }

        [FunctionName("ResetStatistics")]
        public static IActionResult ResetStatistics([HttpTrigger(AuthorizationLevel.Function, "post", Route = "statistics/reset")] HttpRequest req, ILogger log)
        {
            ProcessStatistics.Reset();
            return new OkResult();
        }
    }
}
