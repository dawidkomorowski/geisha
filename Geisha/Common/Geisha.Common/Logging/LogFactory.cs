using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Geisha.Common.Logging
{
    public static class LogFactory
    {
        public static ILog Create(Type type)
        {
            return new Log(LogManager.GetLogger(type.FullName, type));
        }

        public static void ConfigureFileTarget(string filename)
        {
            var loggingConfiguration = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            loggingConfiguration.AddTarget("File", fileTarget);

            fileTarget.FileName = "${basedir}/" + filename;
            fileTarget.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level} ${logger} ${message}";

            var fileTargetRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            loggingConfiguration.LoggingRules.Add(fileTargetRule);

            LogManager.Configuration = loggingConfiguration;
        }
    }
}