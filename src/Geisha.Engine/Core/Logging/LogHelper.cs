using System;
using NLog;
using NLog.Layouts;

namespace Geisha.Engine.Core.Logging
{
    // TODO Global Diagnostics Context could be used to have more detailed logging like include subsystem in which logging takes place (https://github.com/nlog/NLog/wiki/Gdc-Layout-Renderer).
    public static class LogHelper
    {
        public static void ConfigureFileTarget(string filename)
        {
            var layout = new SimpleLayout(@"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level} ${logger} ${message}");
            LogManager.Setup().LoadConfiguration(builder => { builder.ForLogger().FilterMinLevel(NLog.LogLevel.Debug).WriteToFile(filename, layout); });
        }

        public static void SetLogLevel(LogLevel level)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.SetLoggingLevels(Convert(level), NLog.LogLevel.Fatal);
            }

            LogManager.ReconfigExistingLoggers();
        }

        private static NLog.LogLevel Convert(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => NLog.LogLevel.Trace,
                LogLevel.Debug => NLog.LogLevel.Debug,
                LogLevel.Info => NLog.LogLevel.Info,
                LogLevel.Warn => NLog.LogLevel.Warn,
                LogLevel.Error => NLog.LogLevel.Error,
                LogLevel.Fatal => NLog.LogLevel.Fatal,
                LogLevel.Off => NLog.LogLevel.Off,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }
    }
}