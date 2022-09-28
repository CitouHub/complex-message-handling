using Azure.Messaging.ServiceBus.Administration;
using Microsoft.AspNetCore.Mvc;

using CMH.Common.Extenstion;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;

        public QueueController(ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            _serviceBusAdministrationClient = serviceBusAdministrationClient;
        }

        [HttpGet]
        [Route("{queuePrefix}")]
        public async Task<List<string>> GetQueueNames(string queuePrefix)
        {
            return await _serviceBusAdministrationClient.GetQueueNamesAsync(queuePrefix);
        }
    }
}