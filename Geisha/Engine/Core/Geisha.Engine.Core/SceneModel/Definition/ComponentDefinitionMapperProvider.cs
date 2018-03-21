using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Provides functionality to get mapper for given <see cref="IComponent" /> or <see cref="IComponentDefinition" />.
    /// </summary>
    public interface IComponentDefinitionMapperProvider
    {
        /// <summary>
        ///     Returns mapper for given component.
        /// </summary>
        /// <param name="component">Component for which mapper is requested.</param>
        /// <returns>Mapper for given component.</returns>
        IComponentDefinitionMapper GetMapperFor(IComponent component);

        /// <summary>
        ///     Returns mapper for given component definition.
        /// </summary>
        /// <param name="componentDefinition">Component definition for which mapper is requested.</param>
        /// <returns>Mapper for given component definition.</returns>
        IComponentDefinitionMapper GetMapperFor(IComponentDefinition componentDefinition);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to get mapper for given <see cref="T:Geisha.Engine.Core.SceneModel.IComponent" /> or
    ///     <see cref="T:Geisha.Engine.Core.SceneModel.Definition.IComponentDefinition" />.
    /// </summary>
    internal class ComponentDefinitionMapperProvider : IComponentDefinitionMapperProvider
    {
        private readonly IEnumerable<IComponentDefinitionMapper> _componentDefinitionMappers;

        public ComponentDefinitionMapperProvider(IEnumerable<IComponentDefinitionMapper> componentDefinitionMappers)
        {
            _componentDefinitionMappers = componentDefinitionMappers;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given component.
        /// </summary>
        public IComponentDefinitionMapper GetMapperFor(IComponent component)
        {
            var mappers = _componentDefinitionMappers.Where(m => m.ComponentType == component.GetType()).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for component type: {component.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component type is expected.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for component type: {component.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component type is expected.");
            }

            return mappers.Single();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given component definition.
        /// </summary>
        public IComponentDefinitionMapper GetMapperFor(IComponentDefinition componentDefinition)
        {
            var mappers = _componentDefinitionMappers.Where(m => m.ComponentDefinitionType == componentDefinition.GetType()).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for component definition type: {componentDefinition.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component definition type is expected.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for component definition type: {componentDefinition.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component definition type is expected.");
            }

            return mappers.Single();
        }
    }
}