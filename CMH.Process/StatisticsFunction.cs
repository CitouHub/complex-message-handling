using System.Linq;
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using CMH.Data.Model;
using CMH.Process.Util;

namespace CMH.Process
{
    public class StatisticsFunction
    {
        [FunctionName("GetStatistics")]
        public static IActionResult GetStatistics([HttpTrigger(AuthorizationLevel.Function, "get", Route = "statistics")] HttpRequest req, ILogger log)
        {
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMemoryUsage = RuntimeTracker.TotalMemoryUsage,
                TotalMessagesProcessed = RuntimeTracker.TotalMessagesProcessed,
                TotalProcessDuration = RuntimeTracker.TotalProcessDuration,
                MaxParallellTasks = RuntimeTracker.ParallellTasks.Max(),
                AvgParallellTasks = (short)Math.Round(RuntimeTracker.ParallellTasks.Average(_ => _), 0),
                SessionStart = RuntimeTracker.SessionStart,
                SessionStop = RuntimeTracker.SessionStop
            };

            return new OkObjectResult(runtimeStatistics);
        }

        [FunctionName("ResetStatistics")]
        public static IActionResult ResetStatistics([HttpTrigger(AuthorizationLevel.Function, "post", Route = "statistics/reset")] HttpRequest req, ILogger log)
        {
            RuntimeTracker.Reset();
            return new OkResult();
        }
    }
}
