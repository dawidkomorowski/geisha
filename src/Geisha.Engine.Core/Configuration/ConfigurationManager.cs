using System;
using System.Collections.Concurrent;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Is this name good? It conflicts with existing .NET class and actually it is more a provider as it only loads and provides. <- It may not be true. Maybe it should be moved to Geisha.Common and used as well in Editor?
    /// <summary>
    ///     Provides access to configuration.
    /// </summary>
    /// <remarks>
    ///     To provide default values for configuration initialize properties of configuration class in public
    ///     parameterless constructor.
    /// </remarks>
    public interface IConfigurationManager
    {
        /// <summary>
        ///     Returns configuration object of specified type read from configuration file.
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration. It must be a class with public parameterless constructor.</typeparam>
        /// <returns>
        ///     Configuration object with data from configuration file if configuration entry exists in configuration file;
        ///     otherwise default configuration.
        /// </returns>
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new();
    }

    internal sealed class ConfigurationManager : IConfigurationManager
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