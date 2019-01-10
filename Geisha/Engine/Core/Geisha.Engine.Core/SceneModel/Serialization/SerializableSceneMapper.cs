using System.Linq;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Scene" /> and <see cref="SerializableScene" /> in both directions.
    /// </summary>
    public interface ISerializableSceneMapper
    {
        /// <summary>
        ///     Maps <see cref="Scene" /> to <see cref="SerializableScene" />.
        /// </summary>
        /// <param name="scene"><see cref="Scene" /> to be mapped.</param>
        /// <returns><see cref="SerializableScene" /> that is equivalent of given <see cref="Scene" />.</returns>
        SerializableScene MapToSerializable(Scene scene);

        /// <summary>
        ///     Maps <see cref="SerializableScene" /> to <see cref="Scene" />.
        /// </summary>
        /// <param name="serializableScene"><see cref="SerializableScene" /> to be mapped.</param>
        /// <returns><see cref="Scene" /> that is equivalent of given <see cref="SerializableScene" />.</returns>
        Scene MapFromSerializable(SerializableScene serializableScene);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to map between <see cref="Scene" /> and <see cref="SerializableScene" /> in both directions.
    /// </summary>
    internal class SerializableSceneMapper : ISerializableSceneMapper
    {
        private readonly IEntityDefinitionMapper _entityDefinitionMapper;

        public SerializableSceneMapper(IEntityDefinitionMapper entityDefinitionMapper)
        {
            _entityDefinitionMapper = entityDefinitionMapper;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="Scene" /> to <see cref="SerializableScene" />.
        /// </summary>
        public SerializableScene MapToSerializable(Scene scene)
        {
            var serializableScene = new SerializableScene
            {
                RootEntities = scene.RootEntities.Select(e => _entityDefinitionMapper.ToDefinition(e)).ToList()
            };

            return serializableScene;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="SerializableScene" /> to <see cref="Scene" />.
        /// </summary>
        public Scene MapFromSerializable(SerializableScene serializableScene)
        {
            var scene = new Scene();
            foreach (var entityDefinition in serializableScene.RootEntities)
            {
                scene.AddEntity(_entityDefinitionMapper.FromDefinition(entityDefinition));
            }

            return scene;
        }
    }
}