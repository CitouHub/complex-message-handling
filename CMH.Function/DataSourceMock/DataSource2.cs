namespace CMH.Process.DataSourceMock
{
    public class DataSource2 : DataSource
    {
        public DataSource2()
        {
            FailRate = (decimal)0.05;
            MinProcessTime = 200;
            MaxProcessTime = 1200;
        }
    }
}
