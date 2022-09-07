using CMH.Common.Enum;
using CMH.Data.Model;
using CMH.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ProcessChannelPolicyController : ControllerBase
    {
        private readonly IProcessChannelPolicyRepository _processChannelPolicyRepository;

        public ProcessChannelPolicyController(IProcessChannelPolicyRepository processChannelPolicyRepository)
        {
            _processChannelPolicyRepository = processChannelPolicyRepository;
        }

        [HttpGet]
        [Route("")]
        public List<ProcessChannelPolicy> GetAll()
        {
            return _processChannelPolicyRepository.GetAll();
        }

        [HttpGet]
        [Route("{processChannel}")]
        public ProcessChannelPolicy? Get(ProcessChannel processChannel)
        {
            return _processChannelPolicyRepository.Get(processChannel);
        }

        [HttpPut]
        [Route("{processChannel}")]
        public void Update(ProcessChannel processChannel, [FromBody] ProcessChannelPolicy processChannelPolicyUpdate)
        {
            _processChannelPolicyRepository.Update(processChannel, processChannelPolicyUpdate);
        }
    }
}