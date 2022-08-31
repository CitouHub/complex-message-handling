namespace CMH.Process.DataSourceMock
{
    public class DataSource7 : DataSource
    {
        public DataSource7()
        {
            FailRate = (decimal)0.40;
            MinProcessTime = 1000;
            MaxProcessTime = 3000;
        }
    }
}
