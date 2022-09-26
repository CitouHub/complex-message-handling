using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

using CMH.Data.Repository;
using CMH.Priority.Infrastructure;
using CMH.Priority.Service;
using CMH.Test.Helper;

namespace CMH.Test.Service
{
    public class PriorityServiceTest
    {
        private readonly ILogger<PriorityService> _logger = Substitute.For<ILogger<PriorityService>>();
        private readonly IQueueCache _queueCache = Substitute.For<IQueueCache>();
        private readonly IMessageStatisticsRepository _messageStatisticsRepository = Substitute.For<IMessageStatisticsRepository>();
        private readonly IRuntimeStatisticsRepository _runtimeStatisticsRepository = Substitute.For<IRuntimeStatisticsRepository>();
        private readonly IDataSourceRepository _dataSourceRepository = Substitute.For<IDataSourceRepository>();

        private readonly string MockConnectionString = "Endpoint=sb://mock.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=abc123";

        [Theory]
        [InlineData(0, 1, 1, 1, 0)]
        [InlineData(1, 1, 1, 0, 0)]
        [InlineData(0, 0, 1, 1, 1)]
        [InlineData(99, 0, 2, 50, 1)]
        [InlineData(100, 0, 2, 50, 0)]
        [InlineData(101, 0, 2, 50, 0)]
        [InlineData(49, 1, 2, 50, 1)]
        [InlineData(50, 1, 2, 50, 0)]
        [InlineData(51, 1, 2, 50, 0)]
        public void GetAvailableProcessChannelSpots(long messageCount, short priorityIndex, short totalPriorities, short prioritySlots, int expectedAvailableSlots)
        {
            //Setup
            var priorityService = new PriorityService(ConfigHelper.GetConfig(), _logger, 
                new ServiceBusClient(MockConnectionString), new ServiceBusAdministrationClient(MockConnectionString),
                _queueCache, _messageStatisticsRepository, _runtimeStatisticsRepository, _dataSourceRepository);

            //Act
            var availableSlots = priorityService.GetAvailableProcessChannelSpots(messageCount, priorityIndex, totalPriorities, prioritySlots);

            //Assert
            Assert.Equal(expectedAvailableSlots, availableSlots);
        }
    }
}
