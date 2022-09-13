using Geisha.Common.Logging;
using NLog;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Logging
{
    [TestFixture]
    public class LogTests
    {
        private const string LogMessage = "Some log message";
        private ILogger _logger = null!;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Test]
        public void Trace_ShouldDelegateToLoggerTrace()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Trace(LogMessage);

            // Assert
            _logger.Received(1).Trace(LogMessage);
        }

        [Test]
        public void Debug_ShouldDelegateToLoggerDebug()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Debug(LogMessage);

            // Assert
            _logger.Received(1).Debug(LogMessage);
        }

        [Test]
        public void Info_ShouldDelegateToLoggerInfo()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Info(LogMessage);

            // Assert
            _logger.Received(1).Info(LogMessage);
        }

        [Test]
        public void Warn_ShouldDelegateToLoggerWarn()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Warn(LogMessage);

            // Assert
            _logger.Received(1).Warn(LogMessage);
        }

        [Test]
        public void Error_ShouldDelegateToLoggerError()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Error(LogMessage);

            // Assert
            _logger.Received(1).Error(LogMessage);
        }

        [Test]
        public void Fatal_ShouldDelegateToLoggerFatal()
        {
            // Arrange
            var log = GetLog();

            // Act
            log.Fatal(LogMessage);

            // Assert
            _logger.Received(1).Fatal(LogMessage);
        }


        private Log GetLog()
        {
            return new Log(_logger);
        }
    }
}