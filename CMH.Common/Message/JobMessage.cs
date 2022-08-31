namespace CMH.Common.Message
{
    public class JobMessage
    {
        public int JobId { get; set; }
        public short DataSourceId { get; set; }
        public string? JobName { get; set; }
        public string? Description { get; set; }
    }
}
