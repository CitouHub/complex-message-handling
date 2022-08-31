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

        public Task<bool> DoWork()
        {
            var random = new Random();
            var processTime = random.Next(MinProcessTime, MaxProcessTime);

            Thread.Sleep(processTime);

            var success = random.Next(0, 100);
            if(success < FailRate * 100)
            {
                return new Task<bool>(_ => { return false; }, null);
            }

            return new Task<bool>(_ => { return true; }, null);
        }
    }
}
