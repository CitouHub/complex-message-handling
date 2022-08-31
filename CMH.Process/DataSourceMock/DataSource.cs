using System;
using System.Threading;
using System.Threading.Tasks;

namespace CMH.Process.DataSourceMock
{
    public interface IDataSource
    {
        Task<bool> DoWork();
    }

    public class DataSource : IDataSource
    {
        public decimal FailRate { get; set; }

        public int MinProcessTime { get; set; }

        public int MaxProcessTime { get; set; }

        public async Task<bool> DoWork()
        {
            var random = new Random();
            var processTime = random.Next(MinProcessTime, MaxProcessTime);

            var success = random.Next(0, 100);
            var result = success >= FailRate * 100;

            return await Task.Run(async delegate
            {
                await Task.Delay(processTime);
                return result;
            });
        }
    }
}
