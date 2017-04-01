namespace Geisha.Engine.Core.Configuration
{
    public class EngineConfiguration
    {
        public string SystemsConfigurationFileName
            => System.Configuration.ConfigurationManager.AppSettings[nameof(SystemsConfigurationFileName)];
    }
}