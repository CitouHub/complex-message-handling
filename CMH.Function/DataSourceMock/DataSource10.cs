namespace CMH.Process.DataSourceMock
{
    public class DataSource10 : DataSource
    {
        public DataSource10()
        {
            FailRate = (decimal)0.75;
            MinProcessTime = 5000;
            MaxProcessTime = 10000;
        }
    }
}
