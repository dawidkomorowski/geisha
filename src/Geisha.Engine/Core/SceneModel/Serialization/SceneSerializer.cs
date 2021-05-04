using System;
using System.Collections.Generic;
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
            public static class Scene
            {
                public const string SceneBehaviorName = "SceneBehaviorName";
                public const string RootEntities = "RootEntities";
            }

            public static class Entity
            {
                public const string Name = "Name";
            }
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

            jsonWriter.WriteString(PropertyName.Scene.SceneBehaviorName, scene.SceneBehavior.Name);

            WriteRootEntities(jsonWriter, scene.RootEntities);

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

            var sceneBehaviorName = rootElement.GetProperty(PropertyName.Scene.SceneBehaviorName).GetString() ??
                                    throw new InvalidOperationException($"Cannot deserialize scene. {PropertyName.Scene.SceneBehaviorName} property cannot be null.");
            scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(sceneBehaviorName).Create(scene);

            var rootEntitiesElement = rootElement.GetProperty(PropertyName.Scene.RootEntities).EnumerateArray();
            foreach (var entityElement in rootEntitiesElement)
            {
                var entity = new Entity();
                ReadEntity(entityElement, entity);
                scene.AddEntity(entity);
            }

            return scene;
        }

        private void WriteRootEntities(Utf8JsonWriter jsonWriter, IEnumerable<Entity> rootEntities)
        {
            jsonWriter.WriteStartArray(PropertyName.Scene.RootEntities);

            foreach (var entity in rootEntities)
            {
                WriteEntity(jsonWriter, entity);
            }

            jsonWriter.WriteEndArray();
        }

        private void WriteEntity(Utf8JsonWriter jsonWriter, Entity entity)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString(PropertyName.Entity.Name, entity.Name);

            jsonWriter.WriteEndObject();
        }

        private void ReadEntity(JsonElement entityElement, Entity entity)
        {
            var name = entityElement.GetProperty(PropertyName.Entity.Name).GetString() ??
                       throw new InvalidOperationException($"Cannot deserialize scene. {PropertyName.Entity.Name} property of entity cannot be null.");
            entity.Name = name;
        }
    }
}