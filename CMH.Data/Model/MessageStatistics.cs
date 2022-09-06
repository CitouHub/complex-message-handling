namespace CMH.Data.Model
{
    public class MessageStatistics
    {
        public int TotalMessagesHandled { get; set; }
        public int TotalMessagesRescheduled { get; set; }
        public int TotalMessagesDiscarded { get; set; }
        public double TotalMessageDuration { get; set; }
    }
}
