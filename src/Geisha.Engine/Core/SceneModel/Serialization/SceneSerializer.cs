using System.IO;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    internal interface ISceneSerializer
    {
        void Serialize(Scene scene, Stream stream);
        string Serialize(Scene scene);
        Scene Deserialize(Stream stream);
        Scene Deserialize(string json);
    }

    internal sealed class SceneSerializer : ISceneSerializer
    {
        private readonly ISceneFactory _sceneFactory;
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;

        public SceneSerializer(ISceneFactory sceneFactory, ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider)
        {
            _sceneFactory = sceneFactory;
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
        }

        public void Serialize(Scene scene, Stream stream)
        {
        }

        public string Serialize(Scene scene)
        {
            return string.Empty;
        }

        public Scene Deserialize(Stream stream)
        {
            var scene = _sceneFactory.Create();
            scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(string.Empty).Create(scene);
            return scene;
        }

        public Scene Deserialize(string json)
        {
            var scene = _sceneFactory.Create();
            scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(string.Empty).Create(scene);
            return scene;
        }
    }
}