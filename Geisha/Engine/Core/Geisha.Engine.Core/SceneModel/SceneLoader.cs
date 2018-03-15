using System.ComponentModel.Composition;
using Geisha.Common.Serialization;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneLoader
    {
        void Save(Scene scene, string path);
        Scene Load(string path);
    }

    [Export(typeof(ISceneLoader))]
    internal class SceneLoader : ISceneLoader
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISceneDefinitionMapper _sceneDefinitionMapper;

        [ImportingConstructor]
        public SceneLoader(IFileSystem fileSystem, ISceneDefinitionMapper sceneDefinitionMapper)
        {
            _fileSystem = fileSystem;
            _sceneDefinitionMapper = sceneDefinitionMapper;
        }

        public void Save(Scene scene, string path)
        {
            var sceneDefinition = _sceneDefinitionMapper.ToDefinition(scene);
            var serializedSceneDefinition = Serializer.SerializeJson(sceneDefinition);
            _fileSystem.WriteAllTextToFile(path, serializedSceneDefinition);
        }

        public Scene Load(string path)
        {
            var serializedSceneDefinition = _fileSystem.ReadAllTextFromFile(path);
            var sceneDefinition = Serializer.DeserializeJson<SceneDefinition>(serializedSceneDefinition);
            return _sceneDefinitionMapper.FromDefinition(sceneDefinition);
        }
    }
}