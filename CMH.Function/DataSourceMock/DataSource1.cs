namespace CMH.Process.DataSourceMock
{
    public class DataSource1 : DataSource
    {
        public DataSource1()
        {
            FailRate = (decimal)0.01;
            MinProcessTime = 100;
            MaxProcessTime = 1000;
        }
    }
}
