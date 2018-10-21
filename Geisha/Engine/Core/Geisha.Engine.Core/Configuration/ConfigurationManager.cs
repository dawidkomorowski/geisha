using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, IConfiguration;
    }

    internal class ConfigurationManager : IConfigurationManager
    {
        private const string ConfigurationFilePath = "game.json";
        private readonly IEnumerable<IDefaultConfigurationFactory> _defaultConfigurationFactories;
        private readonly IFileSystem _fileSystem;

        public ConfigurationManager(IEnumerable<IDefaultConfigurationFactory> defaultConfigurationFactories, IFileSystem fileSystem)
        {
            _defaultConfigurationFactories = defaultConfigurationFactories;
            _fileSystem = fileSystem;
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, IConfiguration
        {
            var json = _fileSystem.ReadAllTextFromFile(ConfigurationFilePath);
            var gameConfigurationFile = Serializer.DeserializeJson<GameConfigurationFile>(json);

            var configuration = gameConfigurationFile.Configurations.OfType<TConfiguration>().SingleOrDefault();
            var defaultConfigurationFactory = _defaultConfigurationFactories.SingleOrDefault(factory => factory.ConfigurationType == typeof(TConfiguration));

            if (defaultConfigurationFactory == null)
            {
                throw new GeishaEngineException(
                    $"No registered implementation of {nameof(IDefaultConfigurationFactory)} exists for configuration type: {typeof(TConfiguration).Name}.");
            }

            return configuration ?? (TConfiguration) defaultConfigurationFactory.CreateDefault();
        }
    }
}