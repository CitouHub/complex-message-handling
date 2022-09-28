using CMH.Common.Variable;

namespace CMH.Data.Model
{
    public class MessageStatistics
    {
        public int TotalMessagesCompleted { get; set; }
        public int TotalMessagesRescheduled { get; set; }
        public int TotalMessagesDiscarded { get; set; }
        public double TotalMessageDuration { get; set; }
        public DateTime? FirstMessage { get; set; }
        public DateTime? LastMessage { get; set; }

        public void MessageHandled(MessageHandleStatus messageHandleStatus, double duration)
        {
            TotalMessagesCompleted += (messageHandleStatus == MessageHandleStatus.Completed ? 1 : 0);
            TotalMessagesRescheduled += (messageHandleStatus == MessageHandleStatus.Rescheduled ? 1 : 0);
            TotalMessagesDiscarded += (messageHandleStatus == MessageHandleStatus.Discarded ? 1 : 0);
            TotalMessageDuration += (duration < 0 ? 0 : duration);
            FirstMessage = FirstMessage == null ? DateTime.UtcNow : FirstMessage;
            LastMessage = DateTime.UtcNow;
        }
        
        public int TotalMessagesHandled
        {
            get { return TotalMessagesCompleted + TotalMessagesRescheduled + TotalMessagesDiscarded; }
        }

        public double AvgMessageHandleDuration 
        { 
            get { return TotalMessagesHandled > 0 ? Math.Round(TotalMessageDuration / TotalMessagesHandled, 2) : 0; }
        }

        public double AvgThroughPut
        {
            get
            {
                var duration = (LastMessage - FirstMessage)?.TotalMilliseconds;
                return duration != null && duration > 0 ? Math.Round(TotalMessagesHandled / ((double)duration / 1000), 2) : 0;
            }
        }

        public double CompleteRate
        {
            get { return TotalMessagesHandled > 0 ? Math.Round((double)TotalMessagesCompleted / TotalMessagesHandled * 100, 2) : 0; }
        }

        public double RescheduleRate
        {
            get { return TotalMessagesHandled > 0 ? Math.Round((double)TotalMessagesRescheduled / TotalMessagesHandled * 100, 2) : 0; }
        }

        public double DiscardRate
        {
            get { return TotalMessagesHandled > 0 ? Math.Round((double)TotalMessagesDiscarded / TotalMessagesHandled * 100, 2) : 0; }
        }
    }
}
