using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Engine.Core.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class GameTimeProviderTests
    {
        private CoreConfiguration _coreConfiguration;
        private IConfigurationManager _configurationManager;

        [SetUp]
        public void SetUp()
        {
            _coreConfiguration = new CoreConfiguration();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(_coreConfiguration);
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
            var gameTimeProvider = new GameTimeProvider(_configurationManager);

            // Assert
            Assert.That(GameTime.FixedDeltaTime.TotalSeconds, Is.EqualTo(fixedDeltaTimeTotalSeconds).Within(0.001));
        }

        [Test]
        public void GetGameTime_ShouldReturnGameTimeWithZeroDeltaTime_WhenCalledFirstTime()
        {
            // Arrange
            _coreConfiguration.FixedUpdatesPerSecond = 60;
            var gameTimeProvider = new GameTimeProvider(_configurationManager);

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
            _coreConfiguration.FixedUpdatesPerSecond = 60;
            var gameTimeProvider = new GameTimeProvider(_configurationManager);
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
    }
}