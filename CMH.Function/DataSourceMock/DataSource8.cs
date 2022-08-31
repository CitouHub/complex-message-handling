namespace CMH.Process.DataSourceMock
{
    public class DataSource8 : DataSource
    {
        public DataSource8()
        {
            FailRate = (decimal)0.50;
            MinProcessTime = 1500;
            MaxProcessTime = 4000;
        }
    }
}
