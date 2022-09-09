namespace CMH.Data.Model
{
    public class MessageStatistics
    {
        public int TotalMessagesHandled { get; set; }
        public int TotalMessagesRescheduled { get; set; }
        public int TotalMessagesDiscarded { get; set; }
        public double TotalMessageDuration { get; set; }
        
        public double AvgMessageHandleDuration 
        { 
            get { return TotalMessagesHandled > 0 ? Math.Round((double)(TotalMessageDuration / TotalMessagesHandled), 2) : 0; }
        }

        public double RescheduleRate
        {
            get { return TotalMessagesHandled > 0 ? 100 * Math.Round((double)(TotalMessagesRescheduled / TotalMessagesHandled), 2) : 0; }
        }
    }
}
