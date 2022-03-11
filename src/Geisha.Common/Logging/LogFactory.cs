using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Geisha.Common.Logging
{
    // TODO Should Logging be wrapped in custom interface?
    // TODO GetCurrentClassLogger is ok as long as it is used to initialize static readonly field (it happens once per class, not per instance).
    // TODO Global Diagnostics Context could be used to have more detailed logging like include subsystem in which logging takes place (https://github.com/nlog/NLog/wiki/Gdc-Layout-Renderer).
    public static class LogFactory
    {
        public static ILog Create(Type type)
        {
            return new Log(LogManager.GetLogger(type.FullName));
        }

        public static void ConfigureFileTarget(string filename)
        {
            var loggingConfiguration = new LoggingConfiguration();

            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/" + filename,
                Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level} ${logger} ${message}"
            };
            loggingConfiguration.AddTarget("File", fileTarget);

            var fileTargetRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            loggingConfiguration.LoggingRules.Add(fileTargetRule);

            LogManager.Configuration = loggingConfiguration;
        }
    }
}