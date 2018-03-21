using System.ComponentModel.Composition;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Entity" /> and <see cref="EntityDefinition" /> in both directions.
    /// </summary>
    public interface IEntityDefinitionMapper
    {
        /// <summary>
        ///     Maps <see cref="Entity" /> to <see cref="EntityDefinition" />.
        /// </summary>
        /// <param name="entity">Entity to be mapped.</param>
        /// <returns><see cref="EntityDefinition" /> that is equivalent of given entity.</returns>
        EntityDefinition ToDefinition(Entity entity);

        /// <summary>
        ///     Maps <see cref="EntityDefinition" /> to <see cref="Entity" />.
        /// </summary>
        /// <param name="entityDefinition">Entity definition to be mapped.</param>
        /// <returns><see cref="Entity" /> that is equivalent of given entity definition</returns>
        Entity FromDefinition(EntityDefinition entityDefinition);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to map between <see cref="Entity" /> and <see cref="EntityDefinition" /> in both directions.
    /// </summary>
    [Export(typeof(IEntityDefinitionMapper))]
    internal class EntityDefinitionMapper : IEntityDefinitionMapper
    {
        private readonly IComponentDefinitionMapperProvider _componentDefinitionMapperProvider;

        public EntityDefinitionMapper(IComponentDefinitionMapperProvider componentDefinitionMapperProvider)
        {
            _componentDefinitionMapperProvider = componentDefinitionMapperProvider;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="Entity" /> to <see cref="EntityDefinition" />.
        /// </summary>
        public EntityDefinition ToDefinition(Entity entity)
        {
            return new EntityDefinition
            {
                Name = entity.Name,
                Children = entity.Children.Select(ToDefinition).ToList(),
                Components = entity.Components.Select(c =>
                {
                    var componentMapper = _componentDefinitionMapperProvider.GetMapperFor(c);
                    return componentMapper.ToDefinition(c);
                }).ToList()
            };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="EntityDefinition" /> to <see cref="Entity" />.
        /// </summary>
        public Entity FromDefinition(EntityDefinition entityDefinition)
        {
            var entity = new Entity
            {
                Name = entityDefinition.Name
            };

            foreach (var definition in entityDefinition.Children)
            {
                entity.AddChild(FromDefinition(definition));
            }

            foreach (var componentDefinition in entityDefinition.Components)
            {
                var componentMapper = _componentDefinitionMapperProvider.GetMapperFor(componentDefinition);
                entity.AddComponent(componentMapper.FromDefinition(componentDefinition));
            }

            return entity;
        }
    }
}