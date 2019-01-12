using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides functionality to get mapper for given <see cref="IComponent" /> or <see cref="ISerializableComponent" />.
    /// </summary>
    public interface ISerializableComponentMapperProvider
    {
        /// <summary>
        ///     Returns mapper for given component.
        /// </summary>
        /// <param name="component">Component for which mapper is requested.</param>
        /// <returns>Mapper for given component.</returns>
        ISerializableComponentMapper GetMapperFor(IComponent component);

        /// <summary>
        ///     Returns mapper for given serializable component.
        /// </summary>
        /// <param name="serializableComponent">Serializable component for which mapper is requested.</param>
        /// <returns>Mapper for given serializable component.</returns>
        ISerializableComponentMapper GetMapperFor(ISerializableComponent serializableComponent);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to get mapper for given <see cref="IComponent" /> or <see cref="ISerializableComponent" />.
    /// </summary>
    internal sealed class SerializableComponentMapperProvider : ISerializableComponentMapperProvider
    {
        private static readonly ILog Log = LogFactory.Create(typeof(SerializableComponentMapperProvider));
        private readonly IEnumerable<ISerializableComponentMapper> _serializableComponentMappers;

        public SerializableComponentMapperProvider(IEnumerable<ISerializableComponentMapper> serializableComponentMappers)
        {
            _serializableComponentMappers = serializableComponentMappers;

            Log.Info("Discovering serializable component mappers...");

            foreach (var serializableComponentMapper in _serializableComponentMappers)
            {
                Log.Info($"Serializable component mapper found: {serializableComponentMapper}");
            }

            Log.Info("Serializable component mappers discovery completed.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given component.
        /// </summary>
        public ISerializableComponentMapper GetMapperFor(IComponent component)
        {
            var mappers = _serializableComponentMappers.Where(m => m.IsApplicableForComponent(component)).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for component type: {component.GetType()}. Single implementation of {nameof(ISerializableComponentMapper)} per component type is required or component should be marked with {nameof(ComponentDefinitionAttribute)} attribute for automatic serialization.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for component type: {component.GetType()}. Single implementation of {nameof(ISerializableComponentMapper)} per component type is required or component should be marked with {nameof(ComponentDefinitionAttribute)} attribute for automatic serialization.");
            }

            return mappers.Single();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given serializable component.
        /// </summary>
        public ISerializableComponentMapper GetMapperFor(ISerializableComponent serializableComponent)
        {
            var mappers = _serializableComponentMappers.Where(m => m.IsApplicableForSerializableComponent(serializableComponent)).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for serializable component type: {serializableComponent.GetType()}. Single implementation of {nameof(ISerializableComponentMapper)} per serializable component type is required.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for serializable component type: {serializableComponent.GetType()}. Single implementation of {nameof(ISerializableComponentMapper)} per serializable component type is required.");
            }

            return mappers.Single();
        }
    }
}