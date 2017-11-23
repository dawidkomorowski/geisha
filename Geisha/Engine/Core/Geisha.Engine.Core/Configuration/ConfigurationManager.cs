using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, IConfiguration;
        EngineConfiguration GetEngineConfiguration();
    }

    [Export(typeof(IConfigurationManager))]
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IEnumerable<IDefaultConfigurationFactory> _defaultConfigurationFactories;
        private readonly EngineConfiguration _engineConfiguration = new EngineConfiguration();
        private readonly IFileSystem _fileSystem;

        [ImportingConstructor]
        public ConfigurationManager([ImportMany] IEnumerable<IDefaultConfigurationFactory> defaultConfigurationFactories, IFileSystem fileSystem)
        {
            _defaultConfigurationFactories = defaultConfigurationFactories;
            _fileSystem = fileSystem;
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, IConfiguration
        {
            var fileName = _engineConfiguration.SystemsConfigurationFileName;

            var json = _fileSystem.ReadAllTextFromFile(fileName);
            var systemsConfigurations = Serializer.DeserializeJson<SystemsConfigurations>(json);

            var configuration = systemsConfigurations.Configurations.OfType<TConfiguration>().SingleOrDefault();
            var defaultConfigurationFactory = _defaultConfigurationFactories.SingleOrDefault(factory => factory.ConfigurationType == typeof(TConfiguration));

            if (defaultConfigurationFactory == null)
                throw new GeishaEngineException(
                    $"No exported implementation of {nameof(IDefaultConfigurationFactory)} exists for configuration type: {typeof(TConfiguration).Name}.");

            return configuration ?? (TConfiguration) defaultConfigurationFactory.CreateDefault();
        }

        public EngineConfiguration GetEngineConfiguration()
        {
            return _engineConfiguration;
        }
    }
}