using System;
using System.Threading.Tasks;

using CMH.Data.Model;

namespace CMH.Process.Extension
{
    public static class DataSourceExtension
    {
        public static async Task<bool> DoWork(this DataSource dataSource)
        {
            var random = new Random();
            var processTime = random.Next(dataSource.MinProcessTime, dataSource.MaxProcessTime);

            var success = random.Next(0, 100);
            var result = success >= dataSource.FailRate * 100;

            return await Task.Run(async delegate
            {
                await Task.Delay(processTime);
                return result;
            });
        }
    }
}