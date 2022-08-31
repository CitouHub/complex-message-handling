namespace CMH.Process.DataSourceMock
{
    public class DataSource9 : DataSource
    {
        public DataSource9()
        {
            FailRate = (decimal)0.60;
            MinProcessTime = 2000;
            MaxProcessTime = 6000;
        }
    }
}
