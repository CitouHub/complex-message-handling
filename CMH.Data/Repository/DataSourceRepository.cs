using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IDataSourceRepository
    {
        DataSource Add(DataSource dataSource);
        List<DataSource> GetAll();
        DataSource? Get(short id);
        void Update(short id, DataSource dataSourceUpdate);
        void Delete(short id);
    }

    public class DataSourceRepository : IDataSourceRepository
    {
        private readonly List<DataSource> DataSources = new ();

        public DataSource Add(DataSource dataSource)
        {
            dataSource.Id ??= DataSources.Max(_ => _.Id) + 1;
            DataSources.Add(dataSource);
            return DataSources.Last();
        }

        public DataSource? Get(short id)
        {
            return DataSources.FirstOrDefault(x => x.Id == id);
        }

        public List<DataSource> GetAll()
        {
            return DataSources;
        }

        public void Update(short id, DataSource dataSourceUpdate)
        {
            var dataSource = DataSources.FirstOrDefault(x => x.Id == id);
            if (dataSource != null)
            {
                dataSource.FailRate = dataSourceUpdate.FailRate;
                dataSource.MinProcessTime = dataSourceUpdate.MinProcessTime;
                dataSource.MaxProcessTime = dataSourceUpdate.MaxProcessTime;
                dataSource.ProcessChannel = dataSourceUpdate.ProcessChannel;
            }
        }

        public void Delete(short id)
        {
            var dataSource = DataSources.FirstOrDefault(x => x.Id == id);
            if (dataSource != null)
            {
                DataSources.Remove(dataSource);
            }
        }
    }
}
