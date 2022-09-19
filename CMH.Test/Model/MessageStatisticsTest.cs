using CMH.Data.Model;
using Xunit;

namespace CMH.Test.Model
{
    public class MessageStatisticsTest
    {
        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(1000, 0, 0)]
        [InlineData(1000, 1, 1000)]
        [InlineData(1000, 2, 500)]
        [InlineData(1001, 4, 250.25)]
        [InlineData(1000, 6, 166.67)]
        public void AvgMessageHandleDuration(double totalMessageDuration, int totalMessagesHandled, double expectedAvgMessageHandleDuration)
        {
            //Act
            var messageStatistics = new MessageStatistics()
            {
                TotalMessageDuration = totalMessageDuration,
                TotalMessagesHandled = totalMessagesHandled
            };

            //Assert
            Assert.Equal(expectedAvgMessageHandleDuration, messageStatistics.AvgMessageHandleDuration);
        }

        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(1, 1, 100)]
        [InlineData(1, 10, 10)]
        [InlineData(1, 1000, 0.1)]
        [InlineData(8, 9, 88.89)]
        public void RescheduleRate(int totalMessagesRescheduled, int totalMessagesHandled, double expectedRescheduleRate)
        {
            //Act
            var messageStatistics = new MessageStatistics()
            {
                TotalMessagesRescheduled = totalMessagesRescheduled,
                TotalMessagesHandled = totalMessagesHandled
            };

            //Assert
            Assert.Equal(expectedRescheduleRate, messageStatistics.RescheduleRate);
        }
    }
}