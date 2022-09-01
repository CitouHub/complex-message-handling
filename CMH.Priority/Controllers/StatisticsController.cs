using Microsoft.AspNetCore.Mvc;

namespace CMH.Priority.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(ILogger<StatisticsController> logger)
        {
            _logger = logger;
        }
    }
}