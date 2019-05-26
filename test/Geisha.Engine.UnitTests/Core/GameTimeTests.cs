using System;
using Geisha.Common;
using Geisha.Engine.Core;
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
        public void EqualityMembers(int hours1, int hours2, bool isEqual)
        {
            // Arrange
            var gameTime1 = new GameTime(TimeSpan.FromHours(hours1));
            var gameTime2 = new GameTime(TimeSpan.FromHours(hours2));

            // Act
            var equals = gameTime1.Equals(gameTime2);
            var objectEquals = ((object) gameTime1).Equals(gameTime2);
            var getHashCode = gameTime1.GetHashCode() == gameTime2.GetHashCode();
            var equalityOperator = gameTime1 == gameTime2;
            var inequalityOperator = gameTime1 != gameTime2;

            // Assert
            Assert.That(equals, Is.EqualTo(isEqual));
            Assert.That(objectEquals, Is.EqualTo(isEqual));
            Assert.That(getHashCode, Is.EqualTo(isEqual));
            Assert.That(equalityOperator, Is.EqualTo(isEqual));
            Assert.That(inequalityOperator, Is.EqualTo(!isEqual));
        }
    }
}