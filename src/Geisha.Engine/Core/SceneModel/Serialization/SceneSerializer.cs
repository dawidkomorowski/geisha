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

        public SceneSerializer(ISceneFactory sceneFactory)
        {
            _sceneFactory = sceneFactory;
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
            return _sceneFactory.Create();
        }

        public Scene Deserialize(string json)
        {
            return _sceneFactory.Create();
        }
    }
}