using CMH.Common.Variable;

namespace CMH.Data.Model
{
    public class PendingHandledProcessMessage
    {
        public ProcessChannel ProcessChannel { get; set; }
        public MessageHandleStatus MessageHandleStatus { get; set; }
        public double Duration { get; set; }
    }
}
