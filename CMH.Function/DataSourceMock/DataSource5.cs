namespace CMH.Process.DataSourceMock
{
    public class DataSource5 : DataSource
    {
        public DataSource5()
        {
            FailRate = (decimal)0.20;
            MinProcessTime = 600;
            MaxProcessTime = 2200;
        }
    }
}
