using CMH.Common.Variable;
using CMH.Data.Model;
using System;
using Xunit;

namespace CMH.Test.Model
{
    public class MessageStatisticsTest
    {
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, 0, 1)]
        [InlineData(0, 1, 0, 1)]
        [InlineData(0, 0, 1, 1)]
        [InlineData(1, 2, 3, 6)]
        public void TotalMessagesHandled(int messagesCompleted, int messagesRescheduled, int messagesDiscarded, int expectedTotalMessagesHandled)
        {
            //Act
            var timeNow = DateTimeOffset.UtcNow;
            var messageStatistics = new MessageStatistics();
            for(var i = 0; i < messagesCompleted; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Completed, timeNow, timeNow);
            }
            for (var i = 0; i < messagesRescheduled; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Rescheduled, timeNow, timeNow);
            }
            for (var i = 0; i < messagesDiscarded; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Discarded, timeNow, timeNow);
            }

            //Assert
            Assert.Equal(expectedTotalMessagesHandled, messageStatistics.TotalMessagesHandled);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 1, 0, 1, 0, 0)]
        [InlineData(0, 1, 0, 1, 0, 1, 0)]
        [InlineData(1, 1, 0, 0, 0, 0, 1)]
        [InlineData(0, 0, 1, 1, 0, 0, 1)]
        [InlineData(0, 0, 0, 0, 1, 1, 1)]
        [InlineData(1, 1, 2, 2, 3, 3, 2.33)]
        [InlineData(1, 1, 2, 2, 4, 3, 2.43)]
        public void AvgMessageHandleDuration(
            int messagesCompleted, int messagesCompletedDuration, 
            int messagesRescheduled, int messagesRescheduledDuration,
            int messagesDiscarded, int messagesDiscardedDuration,
            double expectedAvgMessageHandleDuration)
        {
            //Act
            var timeNow = DateTimeOffset.UtcNow;
            var messageStatistics = new MessageStatistics();
            for (var i = 0; i < messagesCompleted; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Completed, timeNow, timeNow.AddMilliseconds(messagesCompletedDuration));
            }
            for (var i = 0; i < messagesRescheduled; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Rescheduled, timeNow, timeNow.AddMilliseconds(messagesRescheduledDuration));
            }
            for (var i = 0; i < messagesDiscarded; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Discarded, timeNow, timeNow.AddMilliseconds(messagesDiscardedDuration));
            }

            //Assert
            Assert.Equal(expectedAvgMessageHandleDuration, messageStatistics.AvgMessageHandleDuration);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 0, 100, 0, 0)]
        [InlineData(0, 1, 0, 0, 100, 0)]
        [InlineData(0, 0, 1, 0, 0, 100)]
        [InlineData(1, 1, 0, 50, 50, 0)]
        [InlineData(0, 1, 1, 0, 50, 50)]
        [InlineData(1, 0, 1, 50, 0, 50)]
        [InlineData(1, 1, 1, 33.33, 33.33, 33.33)]
        [InlineData(8, 1, 0, 88.89, 11.11, 0)]
        public void Rate(int messagesCompleted, int messagesRescheduled, int messagesDiscarded, double expectedCompleteRate, double expectedRescheduleRate, double expectedDiscardRate)
        {
            //Act
            var messageStatistics = new MessageStatistics();
            var timeNow = DateTimeOffset.UtcNow;
            for (var i = 0; i < messagesCompleted; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Completed, timeNow, timeNow);
            }
            for (var i = 0; i < messagesRescheduled; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Rescheduled, timeNow, timeNow);
            }
            for (var i = 0; i < messagesDiscarded; i++)
            {
                messageStatistics.MessageHandled(MessageHandleStatus.Discarded, timeNow, timeNow);
            }

            //Assert
            Assert.Equal(expectedCompleteRate, messageStatistics.CompleteRate);
            Assert.Equal(expectedRescheduleRate, messageStatistics.RescheduleRate);
            Assert.Equal(expectedDiscardRate, messageStatistics.DiscardRate);
        }
    }
}