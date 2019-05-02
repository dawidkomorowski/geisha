using System;
using System.Collections.Concurrent;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Is this name good? It conflicts with existing .NET class and actually it is more a provider as it only loads and provides.
    // TODO Add documentation.
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new();
    }

    internal class ConfigurationManager : IConfigurationManager
    {
        private const string ConfigurationFilePath = "game.json";
        private readonly string _configurationJson;
        private readonly ConcurrentDictionary<Type, object> _configurations = new ConcurrentDictionary<Type, object>();
        private readonly IJsonSerializer _jsonSerializer;

        public ConfigurationManager(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
            _configurationJson = fileSystem.GetFile(ConfigurationFilePath).ReadAllText();
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new()
        {
            object DeserializeConfiguration(Type type)
            {
                var configuration = _jsonSerializer.DeserializePart<TConfiguration>(_configurationJson, typeof(TConfiguration).Name);
                return configuration ?? new TConfiguration();
            }

            return (TConfiguration) _configurations.GetOrAdd(typeof(TConfiguration), DeserializeConfiguration);
        }
    }
}