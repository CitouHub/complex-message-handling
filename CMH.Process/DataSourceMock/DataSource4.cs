namespace CMH.Process.DataSourceMock
{
    public class DataSource4 : DataSource
    {
        public DataSource4()
        {
            FailRate = (decimal)0.15;
            MinProcessTime = 400;
            MaxProcessTime = 1800;
        }
    }
}
