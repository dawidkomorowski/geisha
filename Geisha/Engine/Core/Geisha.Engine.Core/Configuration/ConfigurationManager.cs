using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Is this name good? It conflicts with existing .NET class and actually it is more a provider as it only loads and provides.
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new();
    }

    // TODO Maybe simplify game.json structure (user defined section names instead of config types full name)?
    internal class ConfigurationManager : IConfigurationManager
    {
        private const string ConfigurationFilePath = "game.json";
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public ConfigurationManager(IFileSystem fileSystem,
            IJsonSerializer jsonSerializer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new()
        {
            // TODO Maybe load configuration at startup and then serve in-memory config objects instead of live file read
            var json = _fileSystem.GetFile(ConfigurationFilePath).ReadAllText();
            var gameConfigurationFile = _jsonSerializer.Deserialize<GameConfigurationFile>(json);

            var configuration = gameConfigurationFile.Configurations.OfType<TConfiguration>().SingleOrDefault();

            return configuration ?? new TConfiguration();
        }
    }
}