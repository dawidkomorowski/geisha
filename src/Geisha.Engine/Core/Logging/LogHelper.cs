using NLog;
using NLog.Config;
using NLog.Targets;

namespace Geisha.Engine.Core.Logging
{
    // TODO Global Diagnostics Context could be used to have more detailed logging like include subsystem in which logging takes place (https://github.com/nlog/NLog/wiki/Gdc-Layout-Renderer).
    public static class LogHelper
    {
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