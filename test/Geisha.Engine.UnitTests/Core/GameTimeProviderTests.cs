using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Common;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class GameTimeProviderTests
    {
        private CoreConfiguration _coreConfiguration;
        private IConfigurationManager _configurationManager;
        private IDateTimeProvider _dateTimeProvider;

        [SetUp]
        public void SetUp()
        {
            _coreConfiguration = new CoreConfiguration();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(_coreConfiguration);
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();

            _coreConfiguration.FixedUpdatesPerSecond = 60;
        }

        [Test]
        public void Constructor_ShouldInitialize_GameTime_DateTimeProvider()
        {
            // Arrange
            // Act
            var gameTimeProvider = GetGameTimeProvider();

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
            var gameTimeProvider = GetGameTimeProvider();

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
            _coreConfiguration.FixedUpdatesPerSecond = fixedUpdatesPerSecond;

            // Act
            var gameTimeProvider = GetGameTimeProvider();

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
            var gameTime = gameTimeProvider.GetGameTime();

            // Assert
            Assert.That(GameTime.FramesSinceStartUp, Is.EqualTo(1));
        }

        private GameTimeProvider GetGameTimeProvider()
        {
            return new GameTimeProvider(_configurationManager, _dateTimeProvider);
        }
    }
}