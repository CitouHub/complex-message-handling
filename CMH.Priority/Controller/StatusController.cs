using Microsoft.AspNetCore.Mvc;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        [Route("ready")]
        public bool IsReady()
        {
            return true;
        }
    }
}