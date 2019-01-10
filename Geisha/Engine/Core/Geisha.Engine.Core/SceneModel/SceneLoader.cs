using Geisha.Common.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Provides functionality to save and load <see cref="Scene" /> from a file.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        ///     Saves scene to a file.
        /// </summary>
        /// <param name="scene">Scene to be saved.</param>
        /// <param name="path">Path to a file that a scene will be saved to.</param>
        void Save(Scene scene, string path);

        /// <summary>
        ///     Loads scene from a file.
        /// </summary>
        /// <param name="path">Path to a file that a scene will be loaded from.</param>
        /// <returns>Scene loaded from a file.</returns>
        Scene Load(string path);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to save and load <see cref="Scene" /> from a file.
    /// </summary>
    internal class SceneLoader : ISceneLoader
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISceneDefinitionMapper _sceneDefinitionMapper;

        public SceneLoader(IFileSystem fileSystem, ISceneDefinitionMapper sceneDefinitionMapper)
        {
            _fileSystem = fileSystem;
            _sceneDefinitionMapper = sceneDefinitionMapper;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Saves scene to a file.
        /// </summary>
        public void Save(Scene scene, string path)
        {
            var sceneDefinition = _sceneDefinitionMapper.ToDefinition(scene);
            var serializedSceneDefinition = Serializer.SerializeJson(sceneDefinition);
            _fileSystem.WriteAllTextToFile(path, serializedSceneDefinition);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads scene from a file.
        /// </summary>
        public Scene Load(string path)
        {
            var serializedSceneDefinition = _fileSystem.ReadAllTextFromFile(path);
            var sceneDefinition = Serializer.DeserializeJson<SceneDefinition>(serializedSceneDefinition);
            return _sceneDefinitionMapper.FromDefinition(sceneDefinition);
        }
    }
}