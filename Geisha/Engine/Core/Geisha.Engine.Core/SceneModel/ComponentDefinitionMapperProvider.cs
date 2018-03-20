using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    public interface IComponentDefinitionMapperProvider
    {
        IComponentDefinitionMapper GetMapperFor(IComponent component);
        IComponentDefinitionMapper GetMapperFor(IComponentDefinition componentDefinition);
    }

    internal class ComponentDefinitionMapperProvider : IComponentDefinitionMapperProvider
    {
        private readonly IEnumerable<IComponentDefinitionMapper> _componentDefinitionMappers;

        public ComponentDefinitionMapperProvider(IEnumerable<IComponentDefinitionMapper> componentDefinitionMappers)
        {
            _componentDefinitionMappers = componentDefinitionMappers;
        }

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