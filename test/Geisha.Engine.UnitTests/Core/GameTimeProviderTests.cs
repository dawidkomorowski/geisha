using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Common;
using Geisha.Engine.Core;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class GameTimeProviderTests
    {
        private IDateTimeProvider _dateTimeProvider = null!;

        [SetUp]
        public void SetUp()
        {
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        }

        [Test]
        public void Constructor_ShouldInitialize_GameTime_DateTimeProvider()
        {
            // Arrange
            // Act
            GetGameTimeProvider();

            // Assert
            Assert.That(GameTime.DateTimeProvider, Is.EqualTo(_dateTimeProvider));
        }

        [Test]
        public void Constructor_ShouldInitialize_GameTime_StartUpTime()
        {
            // Arrange
            var dateTime = DateTime.Now;
            _dateTimeProvider.Now().Returns(dateTime);

            // Act
            GetGameTimeProvider();

            // Assert
            Assert.That(GameTime.StartUpTime, Is.EqualTo(dateTime));
        }

        [TestCase(1, 1)]
        [TestCase(10, 0.1)]
        [TestCase(60, 1.0d / 60)]
        public void Constructor_ShouldInitialize_GameTime_FixedDeltaTime_AccordingToConfigurationProperty_FixedUpdatesPerSecond(int fixedUpdatesPerSecond,
            double fixedDeltaTimeTotalSeconds)
        {
            // Arrange
            var coreConfiguration = CoreConfiguration.CreateBuilder().WithFixedUpdatesPerSecond(fixedUpdatesPerSecond).Build();

            // Act
            GetGameTimeProvider(coreConfiguration);

            // Assert
            Assert.That(GameTime.FixedDeltaTime.TotalSeconds, Is.EqualTo(fixedDeltaTimeTotalSeconds).Within(0.001));
        }

        [Test]
        public void GetGameTime_ShouldReturnGameTimeWithZeroDeltaTime_WhenCalledFirstTime()
        {
            // Arrange
            var gameTimeProvider = GetGameTimeProvider();

            // Act
            var gameTime = gameTimeProvider.GetGameTime();

            // Assert
            Assert.That(gameTime.DeltaTime, Is.EqualTo(TimeSpan.Zero));
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void GetGameTime_ShouldReturnGameTimeWithDeltaTimeOfAtLeastSleepTimeButLessThanStopwatchElapsedTime_WhenCalledAfterSleepTimeFromPreviousCall(
            int sleepMilliseconds)
        {
            // Arrange
            var gameTimeProvider = GetGameTimeProvider();
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            gameTimeProvider.GetGameTime();
            Thread.Sleep(sleepMilliseconds);
            var gameTime = gameTimeProvider.GetGameTime();
            stopwatch.Stop();

            // Assert
            Assert.That(gameTime.DeltaTime, Is.GreaterThan(TimeSpan.FromMilliseconds(sleepMilliseconds)));
            Assert.That(gameTime.DeltaTime, Is.LessThan(stopwatch.Elapsed));
        }

        [Test]
        public void GetGameTime_ShouldIncrement_GameTime_FramesSinceStartUp_ByOne()
        {
            // Arrange
            var gameTimeProvider = GetGameTimeProvider();

            // Assume
            Assert.That(GameTime.FramesSinceStartUp, Is.Zero);

            // Act
            gameTimeProvider.GetGameTime();

            // Assert
            Assert.That(GameTime.FramesSinceStartUp, Is.EqualTo(1));
        }

        private GameTimeProvider GetGameTimeProvider(CoreConfiguration? configuration = default)
        {
            configuration ??= CoreConfiguration.CreateBuilder().WithFixedUpdatesPerSecond(60).Build();
            return new GameTimeProvider(configuration, _dateTimeProvider);
        }
    }
}