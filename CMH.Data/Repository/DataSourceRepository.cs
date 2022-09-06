using CMH.Common.Enum;
using CMH.Data.Model;

namespace CMH.Data.Repository
{
    public interface IDataSourceRepository
    {
        DataSource Add(DataSource dataSource);
        List<DataSource> GetAll();
        DataSource? Get(short id);
        void Update(short Id, DataSource dataSourceUpdate);
        void Delete(short Id);
    }

    public class DataSourceRepository : IDataSourceRepository
    {
        private readonly List<DataSource> DataSources = new ();

        public DataSource Add(DataSource dataSource)
        {
            dataSource.Id = dataSource.Id ?? DataSources.Max(_ => _.Id) + 1;
            if(DataSources.Any(_ => _.Id == dataSource.Id))
            {
                throw new Exception("DataSource already exists");
            }
            DataSources.Add(dataSource);
            return DataSources.Last();
        }

        public void Delete(short Id)
        {
            var dataSource = DataSources.FirstOrDefault(x => x.Id == Id);
            if (dataSource != null)
            {
                DataSources.Remove(dataSource);
            }
        }

        public DataSource? Get(short id)
        {
            return DataSources.FirstOrDefault(x => x.Id == id);
        }

        public List<DataSource> GetAll()
        {
            return DataSources;
        }

        public void Update(short Id, DataSource dataSourceUpdate)
        {
            var dataSource = DataSources.FirstOrDefault(x => x.Id == Id);
            if (dataSource != null)
            {
                dataSource.FailRate = dataSourceUpdate.FailRate;
                dataSource.MinProcessTime = dataSourceUpdate.MinProcessTime;
                dataSource.MaxProcessTime = dataSourceUpdate.MaxProcessTime;
                dataSource.ProcessChannel = dataSourceUpdate.ProcessChannel;
            }
        }
    }
}
