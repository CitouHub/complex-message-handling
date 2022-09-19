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
        [FunctionName("Statistics")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            return new OkObjectResult(new RuntimeStatistics()
            {
                TotalMemoryUsage = RuntimeTracker.TotalMemoryUsage,
                TotalMessagesProcessed = RuntimeTracker.TotalMessagesProcessed,
                TotalProcessDuration = RuntimeTracker.TotalProcessDuration,
                MaxParallellTasks = RuntimeTracker.MaxParallellTasks,
                SessionStart = RuntimeTracker.SessionStart,
                SessionStop = RuntimeTracker.SessionStop
            });
        }
    }
}
