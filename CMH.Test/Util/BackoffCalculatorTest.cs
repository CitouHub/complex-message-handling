using CMH.Common.Util;
using Xunit;

namespace CMH.Test.Util
{
    public class BackoffCalculatorTest
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(0, 1, 1, 1, 0)]
        [InlineData(1, 1, 1, 0, 0)]
        [InlineData(1, 0, 1, 1, 1)]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(10, 1, 10, int.MaxValue, 10)]
        [InlineData(2, 2, 4, int.MaxValue, 16)]
        [InlineData(2, 2, 4, 15, 15)]
        [InlineData(-2, 2, 4, int.MaxValue, 0)]
        public void CalculateIterationSleepTime(int initialSleepTime, double backoffFactor, int iterations, int maxSleepTime, int expectedSleepTime)
        {
            //Act
            var sleepTime = BackoffCalculator.CalculateIterationSleepTime(initialSleepTime, backoffFactor, iterations, maxSleepTime);

            //Assert
            Assert.Equal(expectedSleepTime, sleepTime);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0)]
        [InlineData(0, 1, 1, 1, 1, 1, 0)]
        [InlineData(1, 0, 1, 1, 1, 1, 0)]
        [InlineData(1, 1, 0, 1, 1, 1, 0)]
        [InlineData(1, 1, 1, 0, 1, 1, 0)]
        [InlineData(1, 1, 1, 1, 1, 0, 0)]
        [InlineData(1, 1, 1, 1, 1, 1, 1)]
        [InlineData(-1, 1, 1, 1, 1, 1, 0)]
        [InlineData(1, -1, 1, 1, 1, 1, 0)]
        [InlineData(1, 1, -1, 1, 1, 1, 0)]
        [InlineData(1, 1, 1, -1, 1, 1, 0)]
        [InlineData(1, 1, 1, 1, -1, 1, 0)]
        [InlineData(1, 1, 1, 1, 1, -1, 0)]
        [InlineData(2, 3, 5, 4, 0, int.MaxValue, 120)]
        [InlineData(2, 3, 5, 4, 1, 119, 119)]
        public void CalculatePriorityRescheduleSleepTime(int initialSleepTime, double tryFactor, double priorityFactor, int tries, short priorityIndex, int maxSleepTime, int expectedSleepTime)
        {
            //Act
            var sleepTime = BackoffCalculator.CalculatePriorityRescheduleSleepTime(initialSleepTime, tryFactor, priorityFactor, tries, priorityIndex, maxSleepTime);

            //Assert
            Assert.Equal(expectedSleepTime, sleepTime);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 1, 1, 0)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(-1, 1, 1, 0)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 3, 4, 162)]
        public void CalculateProcessRescheduleSleepTime(int initialSleepTime, double backoffFactor, int tries, int expectedSleepTime)
        {
            //Act
            var sleepTime = BackoffCalculator.CalculateProcessRescheduleSleepTime(initialSleepTime, backoffFactor, tries);

            //Assert
            Assert.Equal(expectedSleepTime, sleepTime);
        }
    }
}