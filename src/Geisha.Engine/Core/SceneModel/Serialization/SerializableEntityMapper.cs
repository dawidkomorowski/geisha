using System.Linq;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Entity" /> and <see cref="SerializableEntity" /> in both
    ///     directions.
    /// </summary>
    public interface ISerializableEntityMapper
    {
        /// <summary>
        ///     Maps <see cref="Entity" /> to <see cref="SerializableEntity" />.
        /// </summary>
        /// <param name="entity"><see cref="Entity" /> to be mapped.</param>
        /// <returns><see cref="SerializableEntity" /> that is equivalent of given <see cref="Entity" />.</returns>
        SerializableEntity MapToSerializable(Entity entity);

        /// <summary>
        ///     Maps <see cref="SerializableEntity" /> to <see cref="Entity" />.
        /// </summary>
        /// <param name="serializableEntity"><see cref="SerializableEntity" /> to be mapped.</param>
        /// <returns><see cref="Entity" /> that is equivalent of given <see cref="SerializableEntity" />.</returns>
        Entity MapFromSerializable(SerializableEntity serializableEntity);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to map between <see cref="Entity" /> and <see cref="SerializableEntity" /> in both
    ///     directions.
    /// </summary>
    internal class SerializableEntityMapper : ISerializableEntityMapper
    {
        private readonly ISerializableComponentMapperProvider _serializableComponentMapperProvider;

        public SerializableEntityMapper(ISerializableComponentMapperProvider serializableComponentMapperProvider)
        {
            _serializableComponentMapperProvider = serializableComponentMapperProvider;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="Entity" /> to <see cref="SerializableEntity" />.
        /// </summary>
        public SerializableEntity MapToSerializable(Entity entity)
        {
            return new SerializableEntity
            {
                Name = entity.Name,
                Children = entity.Children.Select(MapToSerializable).ToList(),
                Components = entity.Components.Select(c =>
                {
                    var componentMapper = _serializableComponentMapperProvider.GetMapperFor(c);
                    return componentMapper.MapToSerializable(c);
                }).ToList()
            };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="SerializableEntity" /> to <see cref="Entity" />.
        /// </summary>
        public Entity MapFromSerializable(SerializableEntity serializableEntity)
        {
            var entity = new Entity
            {
                Name = serializableEntity.Name
            };

            foreach (var serializable in serializableEntity.Children)
            {
                entity.AddChild(MapFromSerializable(serializable));
            }

            foreach (var serializableComponent in serializableEntity.Components)
            {
                var componentMapper = _serializableComponentMapperProvider.GetMapperFor(serializableComponent);
                entity.AddComponent(componentMapper.MapFromSerializable(serializableComponent));
            }

            return entity;
        }
    }
}