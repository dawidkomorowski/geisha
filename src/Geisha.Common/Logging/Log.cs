using NLog;

namespace Geisha.Common.Logging
{
    /// <summary>
    ///     Provides logging interface.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        ///     Writes the diagnostic message at the Trace level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Trace(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Debug(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Info(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Warn level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Warn(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Error(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Fatal(string message);
    }

    // TODO There should be methods with parameters as otherwise dynamic string allocation happens even when log level is disabled.
    internal class Log : ILog
    {
        private readonly ILogger _logger;

        public Log(ILogger logger)
        {
            _logger = logger;
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}