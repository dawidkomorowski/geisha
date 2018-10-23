﻿using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;

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
    ///     Provides functionality to get mapper for given <see cref="IComponent" /> or <see cref="IComponentDefinition" />.
    /// </summary>
    internal class ComponentDefinitionMapperProvider : IComponentDefinitionMapperProvider
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ComponentDefinitionMapperProvider));
        private readonly IEnumerable<IComponentDefinitionMapper> _componentDefinitionMappers;

        public ComponentDefinitionMapperProvider(IEnumerable<IComponentDefinitionMapper> componentDefinitionMappers)
        {
            _componentDefinitionMappers = componentDefinitionMappers;

            Log.Info("Discovering component definition mappers...");

            foreach (var componentDefinitionMapper in _componentDefinitionMappers)
            {
                Log.Info($"Component definition mapper found: {componentDefinitionMapper}");
            }

            Log.Info("Component definition mappers discovery completed.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given component.
        /// </summary>
        public IComponentDefinitionMapper GetMapperFor(IComponent component)
        {
            var mappers = _componentDefinitionMappers.Where(m => m.IsApplicableForComponent(component)).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for component type: {component.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component type is required or component should be marked with {nameof(ComponentDefinitionAttribute)} attribute for automatic serialization.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for component type: {component.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component type is required or component should be marked with {nameof(ComponentDefinitionAttribute)} attribute for automatic serialization.");
            }

            return mappers.Single();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns mapper for given component definition.
        /// </summary>
        public IComponentDefinitionMapper GetMapperFor(IComponentDefinition componentDefinition)
        {
            var mappers = _componentDefinitionMappers.Where(m => m.IsApplicableForComponentDefinition(componentDefinition)).ToList();

            if (mappers.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No mapper found for component definition type: {componentDefinition.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component definition type is required.");
            }

            if (mappers.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple mappers found for component definition type: {componentDefinition.GetType()}. Single implementation of {nameof(IComponentDefinitionMapper)} per component definition type is required.");
            }

            return mappers.Single();
        }
    }
}