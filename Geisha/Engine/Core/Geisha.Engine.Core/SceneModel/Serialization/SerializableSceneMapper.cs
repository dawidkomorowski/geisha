using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<ISceneConstructionScript> _sceneConstructionScripts;
        private readonly ISerializableEntityMapper _serializableEntityMapper;

        public SerializableSceneMapper(ISerializableEntityMapper serializableEntityMapper, IEnumerable<ISceneConstructionScript> sceneConstructionScripts)
        {
            _serializableEntityMapper = serializableEntityMapper;
            _sceneConstructionScripts = sceneConstructionScripts;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="Scene" /> to <see cref="SerializableScene" />.
        /// </summary>
        public SerializableScene MapToSerializable(Scene scene)
        {
            var serializableScene = new SerializableScene
            {
                RootEntities = scene.RootEntities.Select(e => _serializableEntityMapper.MapToSerializable(e)).ToList(),
                ConstructionScriptName = scene.ConstructionScript.Name
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
            foreach (var serializableEntity in serializableScene.RootEntities)
            {
                scene.AddEntity(_serializableEntityMapper.MapFromSerializable(serializableEntity));
            }

            var matchingConstructionScripts = _sceneConstructionScripts.Where(s => s.Name == serializableScene.ConstructionScriptName).ToList();
            if (matchingConstructionScripts.Count == 1)
            {
                scene.ConstructionScript = matchingConstructionScripts.Single();
            }
            else
            {
                throw new InvalidOperationException(
                    $"There must be exactly one {nameof(ISceneConstructionScript)} implementation registered with name: {serializableScene.ConstructionScriptName}");
            }

            return scene;
        }
    }
}