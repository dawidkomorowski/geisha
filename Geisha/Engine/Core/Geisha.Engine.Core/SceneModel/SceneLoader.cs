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
        private readonly ISerializableSceneMapper _serializableSceneMapper;

        public SceneLoader(IFileSystem fileSystem, ISerializableSceneMapper serializableSceneMapper)
        {
            _fileSystem = fileSystem;
            _serializableSceneMapper = serializableSceneMapper;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Saves scene to a file.
        /// </summary>
        public void Save(Scene scene, string path)
        {
            var serializableScene = _serializableSceneMapper.MapToSerializable(scene);
            var serializedSerializableScene = Serializer.SerializeJson(serializableScene);
            _fileSystem.WriteAllTextToFile(path, serializedSerializableScene);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads scene from a file.
        /// </summary>
        public Scene Load(string path)
        {
            var serializedSerializableScene = _fileSystem.GetFile(path).ReadAllText();
            var serializableScene = Serializer.DeserializeJson<SerializableScene>(serializedSerializableScene);
            return _serializableSceneMapper.MapFromSerializable(serializableScene);
        }
    }
}