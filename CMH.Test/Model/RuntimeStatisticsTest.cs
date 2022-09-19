using CMH.Common.Variable;
using CMH.Data.Model;
using System;
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

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(2000, 2, 1)]
        [InlineData(1000, 10, 10)]
        [InlineData(6000, 10, 1.67)]
        public void AvgThroughPut(int sessionDuration, int totalMessagesProcessed, double expectedAvgThroughPut)
        {
            //Act
            var now = DateTimeOffset.UtcNow;
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMessagesProcessed = totalMessagesProcessed,
                SessionStart = now.AddMilliseconds(-1*sessionDuration),
                SessionStop = now
            };

            //Assert
            Assert.Equal(expectedAvgThroughPut, runtimeStatistics.AvgThroughPut);
        }

        [Fact]
        public void AvgThroughPut_NoStart()
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMessagesProcessed = 1,
                SessionStop = DateTimeOffset.UtcNow
            };

            //Assert
            Assert.Equal(0, runtimeStatistics.AvgThroughPut);
        }

        [Fact]
        public void AvgThroughPut_NoStop()
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMessagesProcessed = 1,
                SessionStart = DateTimeOffset.UtcNow
            };

            //Assert
            Assert.Equal(0, runtimeStatistics.AvgThroughPut);
        }

        [Fact]
        public void AvgThroughPut_NoSession()
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalMessagesProcessed = 1
            };

            //Assert
            Assert.Equal(0, runtimeStatistics.AvgThroughPut);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1000, 1, 1)]
        [InlineData(10000, 2, 0.2)]
        public void ApproxExecutionTimeCost(double totalProcessDuration, long totalGBMemoryUsage, double expectedGBs)
        {
            //Act
            var runtimeStatistics = new RuntimeStatistics()
            {
                TotalProcessDuration = totalProcessDuration,
                TotalMemoryUsage = (long)(totalGBMemoryUsage * Economy.Function.ByteToGigaByteFactor)
            };

            //Assert
            Assert.Equal(expectedGBs * Economy.Function.GBsCost, runtimeStatistics.ApproxExecutionTimeCost);
        }
    }
}