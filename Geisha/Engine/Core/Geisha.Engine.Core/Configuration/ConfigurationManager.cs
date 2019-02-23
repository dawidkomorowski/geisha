using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Is this name good? It conflicts with existing .NET class and actually it is more a provider as it only loads and provides.
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, IConfiguration;
    }

    // TODO Maybe simplify game.json structure (user defined section names instead of config types full name)?
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
            // TODO Maybe load configuration at startup and then serve in-memory config objects instead of live file read
            var json = _fileSystem.GetFile(ConfigurationFilePath).ReadAllText();
            var gameConfigurationFile = Serializer.DeserializeJson<GameConfigurationFile>(json);

            var configuration = gameConfigurationFile.Configurations.OfType<TConfiguration>().SingleOrDefault();
            var defaultConfigurationFactory = _defaultConfigurationFactories.SingleOrDefault(factory => factory.ConfigurationType == typeof(TConfiguration));

            // TODO Are these default configuration factories actually useful? Maybe default constructor of config class should handle that?
            if (defaultConfigurationFactory == null)
            {
                throw new GeishaEngineException(
                    $"No registered implementation of {nameof(IDefaultConfigurationFactory)} exists for configuration type: {typeof(TConfiguration).Name}.");
            }

            return configuration ?? (TConfiguration) defaultConfigurationFactory.CreateDefault();
        }
    }
}