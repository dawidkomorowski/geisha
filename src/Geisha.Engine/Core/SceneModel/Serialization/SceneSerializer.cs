using System;
using System.IO;
using System.Text;
using System.Text.Json;

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
        private static class PropertyName
        {
            public const string SceneBehaviorName = "SceneBehaviorName";
        }

        private readonly ISceneFactory _sceneFactory;
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;

        public SceneSerializer(ISceneFactory sceneFactory, ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider)
        {
            _sceneFactory = sceneFactory;
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
        }

        public void Serialize(Scene scene, Stream stream)
        {
            using var jsonWriter = new Utf8JsonWriter(stream);
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString(PropertyName.SceneBehaviorName, scene.SceneBehavior.Name);

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        public string Serialize(Scene scene)
        {
            using var memoryStream = new MemoryStream();
            Serialize(scene, memoryStream);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public Scene Deserialize(Stream stream)
        {
            using var jsonDocument = JsonDocument.Parse(stream);
            return DeserializeInternal(jsonDocument);
        }

        public Scene Deserialize(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            return DeserializeInternal(jsonDocument);
        }

        private Scene DeserializeInternal(JsonDocument jsonDocument)
        {
            var rootElement = jsonDocument.RootElement;
            var scene = _sceneFactory.Create();

            var sceneBehaviorName = rootElement.GetProperty(PropertyName.SceneBehaviorName).GetString() ??
                                    throw new InvalidOperationException($"Cannot deserialize scene. {PropertyName.SceneBehaviorName} property cannot be null.");
            scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(sceneBehaviorName).Create(scene);

            return scene;
        }
    }
}