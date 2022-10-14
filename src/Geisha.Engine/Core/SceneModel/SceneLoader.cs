using System.IO;
using Geisha.Engine.Core.SceneModel.Serialization;
using NLog;

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

    internal class SceneLoader : ISceneLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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

        public Scene Load(string path)
        {
            Logger.Info("Loading scene from file: {0}", path);

            var serializedScene = File.ReadAllText(path);
            var scene = _sceneSerializer.Deserialize(serializedScene);

            Logger.Info("Scene loaded successfully.");

            return scene;
        }
    }
}