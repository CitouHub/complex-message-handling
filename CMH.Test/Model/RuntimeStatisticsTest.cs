using CMH.Data.Model;
using Xunit;

namespace CMH.Test.Model
{
    public class RuntimeStatisticsTest
    {
        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(10, 5, 2)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 6, 1.67)]
        public void AvgMessagesPerQuery(int totalMessagesFetched, int priorityQueueQueries, double expectedAvgMessagesPerQuery)
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMessagesFetched = totalMessagesFetched,
                PriorityQueueQueries = priorityQueueQueries
            };

            //Assert
            Assert.Equal(expectedAvgMessagesPerQuery, runtimeStatistics.AvgMessagesPerQuery);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(10, 5, 2)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 6, 1.67)]
        public void AvgMessagesFetchDuration(double totalFetchDuration, int totalMessagesFetched, double expectedAvgMessagesFetchDuration)
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalFetchDuration = totalFetchDuration,
                TotalMessagesFetched = totalMessagesFetched,
            };

            //Assert
            Assert.Equal(expectedAvgMessagesFetchDuration, runtimeStatistics.AvgMessagesFetchDuration);
        }
    }
}