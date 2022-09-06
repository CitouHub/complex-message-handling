using CMH.Common.Enum;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IProcessChannelPolicyRepository
    {
        ProcessChannelPolicy Add(ProcessChannelPolicy processChannelPolicy);
        List<ProcessChannelPolicy> GetAll();
        ProcessChannelPolicy? Get(ProcessChannel processChannel);
        void Update(ProcessChannel processChannel, ProcessChannelPolicy processChannelPolicy);
    }

    public class ProcessChannelPolicyRepository : IProcessChannelPolicyRepository
    {
        private readonly List<ProcessChannelPolicy> _processChannelPolicies = new();

        public ProcessChannelPolicy Add(ProcessChannelPolicy processChannelPolicy)
        {
            _processChannelPolicies.Add(processChannelPolicy);
            return _processChannelPolicies.Last();
        }

        public ProcessChannelPolicy Get(ProcessChannel processChannel)
        {
            return _processChannelPolicies.First(x => x.Name == processChannel.ToString());
        }

        public List<ProcessChannelPolicy> GetAll()
        {
            return _processChannelPolicies;
        }

        public void Update(ProcessChannel processChannel, ProcessChannelPolicy processChannelPolicyUpdate)
        {
            var processChannelPolicy = _processChannelPolicies.FirstOrDefault(x => x.Name == processChannel.ToString());
            if (processChannelPolicy != null)
            {
                processChannelPolicy.Tries = processChannelPolicyUpdate.Tries;
                processChannelPolicy.InitialSleepTime = processChannelPolicyUpdate.InitialSleepTime;
                processChannelPolicy.BackoffFactor = processChannelPolicyUpdate.BackoffFactor;
            }
        }
    }
}
