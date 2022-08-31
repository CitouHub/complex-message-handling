namespace CMH.Process.DataSourceMock
{
    public class DataSource6 : DataSource
    {
        public DataSource6()
        {
            FailRate = (decimal)0.30;
            MinProcessTime = 800;
            MaxProcessTime = 2600;
        }
    }
}
