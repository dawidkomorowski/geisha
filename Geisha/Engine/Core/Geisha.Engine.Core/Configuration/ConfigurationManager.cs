using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;

namespace Geisha.Engine.Core.Configuration
{
    [Export(typeof(IConfigurationManager))]
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly EngineConfiguration _engineConfiguration = new EngineConfiguration();
        private readonly IEnumerable<IDefaultConfigurationFactory> _defaultConfigurationFactories;

        [ImportingConstructor]
        public ConfigurationManager([ImportMany] IEnumerable<IDefaultConfigurationFactory> defaultConfigurationFactories)
        {
            _defaultConfigurationFactories = defaultConfigurationFactories;
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class
        {
            var fileName = _engineConfiguration.SystemsConfigurationFileName;

            var json = File.ReadAllText(fileName); // TODO file io abstraction is needed to test this class and to have platform independency
            var systemsConfigurations = Serializer.DeserializeJson<SystemsConfigurations>(json);

            var configuration = systemsConfigurations.Configurations.OfType<TConfiguration>().SingleOrDefault();
            var defaultConfigurationFactory = _defaultConfigurationFactories.Single(factory => factory.ConfigurationType == typeof(TConfiguration));

            return configuration ?? (TConfiguration) defaultConfigurationFactory.CreateDefault();
        }

        public EngineConfiguration GetEngineConfiguration()
        {
            return _engineConfiguration;
        }
    }
}