using System.IO;
using Geisha.Common.Logging;
using Geisha.Engine.Core.SceneModel.Serialization;

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
        ///     Saves scene to a stream.
        /// </summary>
        /// <param name="scene">Scene to be saved.</param>
        /// <param name="stream">Stream that a scene will be saved to.</param>
        void Save(Scene scene, Stream stream);

        /// <summary>
        ///     Loads scene from a file.
        /// </summary>
        /// <param name="path">Path to a file that a scene will be loaded from.</param>
        /// <returns>Scene loaded from a file.</returns>
        Scene Load(string path);
    }

    internal class SceneLoader : ISceneLoader
    {
        private static readonly ILog Log = LogFactory.Create(typeof(SceneLoader));
        private readonly ISceneSerializer _sceneSerializer;

        public SceneLoader(ISceneSerializer sceneSerializer)
        {
            _sceneSerializer = sceneSerializer;
        }

        public void Save(Scene scene, string path)
        {
            var serializedScene = _sceneSerializer.Serialize(scene);
            File.WriteAllText(path, serializedScene);
        }

        // TODO Remove this and make SceneLoader to be a serializer over a file system (SceneFileLoader).???
        public void Save(Scene scene, Stream stream)
        {
            _sceneSerializer.Serialize(scene, stream);
        }

        public Scene Load(string path)
        {
            Log.Debug($"Loading scene from file: {path}");

            var serializedScene = File.ReadAllText(path);
            var scene = _sceneSerializer.Deserialize(serializedScene);

            Log.Debug("Scene loaded successfully.");

            return scene;
        }
    }
}