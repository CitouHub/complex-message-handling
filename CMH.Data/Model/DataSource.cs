using CMH.Common.Enum;

namespace CMH.Data.Model
{
    public class DataSource
    {
        public short? Id { get; set; }

        public double FailRate { get; set; }

        public int MinProcessTime { get; set; }

        public int MaxProcessTime { get; set; }    
        
        public ProcessChannel ProcessChannel { get; set; }

        public string Description { 
            get { return $"{Id}, Fail: {FailRate * 100} %, Avg. time: {(MaxProcessTime - MinProcessTime) / 2 + MinProcessTime} ms, PC: {ProcessChannel}"; }
        }
    }
}