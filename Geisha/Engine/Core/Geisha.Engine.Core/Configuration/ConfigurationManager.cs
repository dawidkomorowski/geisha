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
        private readonly GameConfigurationFile _gameConfigurationFile;

        public ConfigurationManager(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            var json = fileSystem.GetFile(ConfigurationFilePath).ReadAllText();
            _gameConfigurationFile = jsonSerializer.Deserialize<GameConfigurationFile>(json);
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new()
        {
            var configuration = _gameConfigurationFile.Configurations.OfType<TConfiguration>().SingleOrDefault();

            return configuration ?? new TConfiguration();
        }
    }
}