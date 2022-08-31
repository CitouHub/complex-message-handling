namespace CMH.Process.DataSourceMock
{
    public class DataSource3 : DataSource
    {
        public DataSource3()
        {
            FailRate = (decimal)0.10;
            MinProcessTime = 300;
            MaxProcessTime = 1600;
        }
    }
}
