using System.ComponentModel.Composition;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Scene" /> and <see cref="SceneDefinition" /> in both directions.
    /// </summary>
    public interface ISceneDefinitionMapper
    {
        /// <summary>
        ///     Maps <see cref="Scene" /> to <see cref="SceneDefinition" />.
        /// </summary>
        /// <param name="scene">Scene to be mapped.</param>
        /// <returns><see cref="SceneDefinition" /> that is equivalent of given scene.</returns>
        SceneDefinition ToDefinition(Scene scene);

        /// <summary>
        ///     Maps <see cref="SceneDefinition" /> to <see cref="Scene" />.
        /// </summary>
        /// <param name="sceneDefinition">Scene definition to be mapped.</param>
        /// <returns><see cref="Scene" /> that is equivalent of given scene definition.</returns>
        Scene FromDefinition(SceneDefinition sceneDefinition);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to map between <see cref="Scene" /> and <see cref="SceneDefinition" /> in both directions.
    /// </summary>
    [Export(typeof(ISceneDefinitionMapper))]
    internal class SceneDefinitionMapper : ISceneDefinitionMapper
    {
        private readonly IEntityDefinitionMapper _entityDefinitionMapper;

        [ImportingConstructor]
        public SceneDefinitionMapper(IEntityDefinitionMapper entityDefinitionMapper)
        {
            _entityDefinitionMapper = entityDefinitionMapper;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="Scene" /> to <see cref="SceneDefinition" />.
        /// </summary>
        public SceneDefinition ToDefinition(Scene scene)
        {
            var sceneDefinition = new SceneDefinition
            {
                RootEntities = scene.RootEntities.Select(e => _entityDefinitionMapper.ToDefinition(e)).ToList()
            };

            return sceneDefinition;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="SceneDefinition" /> to <see cref="Scene" />.
        /// </summary>
        public Scene FromDefinition(SceneDefinition sceneDefinition)
        {
            var scene = new Scene();
            foreach (var entityDefinition in sceneDefinition.RootEntities)
            {
                scene.AddEntity(_entityDefinitionMapper.FromDefinition(entityDefinition));
            }

            return scene;
        }
    }
}