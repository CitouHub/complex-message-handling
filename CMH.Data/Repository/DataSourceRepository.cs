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
        private readonly List<DataSource> _dataSources = new();

        public DataSource Add(DataSource dataSource)
        {
            dataSource.Id ??= _dataSources.Max(_ => _.Id) + 1;
            _dataSources.Add(dataSource);
            return _dataSources.Last();
        }

        public DataSource? Get(short id)
        {
            return _dataSources.FirstOrDefault(x => x.Id == id);
        }

        public List<DataSource> GetAll()
        {
            return _dataSources;
        }

        public void Update(short id, DataSource dataSourceUpdate)
        {
            var dataSource = _dataSources.FirstOrDefault(x => x.Id == id);
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
            var dataSource = _dataSources.FirstOrDefault(x => x.Id == id);
            if (dataSource != null)
            {
                _dataSources.Remove(dataSource);
            }
        }
    }
}
