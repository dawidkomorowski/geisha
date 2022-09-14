using System;
using Geisha.Engine.Core;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class GameTimeTests
    {
        [Test]
        public void TimeSinceStartUp_ShouldReturnDifferenceBetweenCurrentTimeAndStartUpTime()
        {
            // Arrange
            var currentTime = DateTime.Now;
            var startUpTime = currentTime.AddHours(-1);

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.Now().Returns(currentTime);

            GameTime.StartUpTime = startUpTime;
            GameTime.DateTimeProvider = dateTimeProvider;

            // Act
            var actual = GameTime.TimeSinceStartUp;

            // Assert
            Assert.That(actual, Is.EqualTo(currentTime - startUpTime));
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void EqualityMembers_ShouldEqualGameTime_WhenDeltaTimeIsEqual(int hours1, int hours2, bool expectedIsEqual)
        {
            // Arrange
            var gameTime1 = new GameTime(TimeSpan.FromHours(hours1));
            var gameTime2 = new GameTime(TimeSpan.FromHours(hours2));

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(gameTime1, gameTime2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }
    }
}