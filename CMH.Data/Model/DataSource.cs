using CMH.Common.Enum;

namespace CMH.Data.Model
{
    public class DataSource
    {
        public short Id { get; set;
        }
        public double FailRate { get; set; }

        public int MinProcessTime { get; set; }

        public int MaxProcessTime { get; set; }    
        
        public ProcessChannel ProcessChannel { get; set; }
    }
}