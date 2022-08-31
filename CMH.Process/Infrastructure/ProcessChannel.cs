namespace CMH.Infrastructure
{
    public class ProcessChannel
    {
        public string Name { get; set; } = "";
        public short Tries { get; set; }
        public short InitialSleepTime { get; set; }
        public double BackoffFactor { get; set; }
    }
}
