using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;

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
                public const string Components = "Components";
                public const string Children = "Children";
            }

            public static class Component
            {
                public const string ComponentId = "ComponentId";
                public const string ComponentData = "ComponentData";
            }

            public static class Vector2
            {
                public const string X = "X";
                public const string Y = "Y";
            }

            public static class Vector3
            {
                public const string X = "X";
                public const string Y = "Y";
                public const string Z = "Z";
            }
        }

        private readonly ISceneFactory _sceneFactory;
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;
        private readonly IComponentFactoryProvider _componentFactoryProvider;
        private readonly IAssetStore _assetStore;

        public SceneSerializer(ISceneFactory sceneFactory, ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider,
            IComponentFactoryProvider componentFactoryProvider, IAssetStore assetStore)
        {
            _sceneFactory = sceneFactory;
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
            _componentFactoryProvider = componentFactoryProvider;
            _assetStore = assetStore;
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

            #region SceneBehaviorName

            var sceneBehaviorName = rootElement.GetProperty(PropertyName.Scene.SceneBehaviorName).GetString() ??
                                    throw new InvalidOperationException(
                                        $"Cannot deserialize scene. {PropertyName.Scene.SceneBehaviorName} property cannot be null.");
            scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(sceneBehaviorName).Create(scene);

            #endregion

            #region RootEntities

            var rootEntitiesElement = rootElement.GetProperty(PropertyName.Scene.RootEntities).EnumerateArray();
            foreach (var entityElement in rootEntitiesElement)
            {
                var entity = new Entity();
                ReadEntity(entityElement, entity);
                scene.AddEntity(entity);
            }

            #endregion

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

            #region Components

            jsonWriter.WriteStartArray(PropertyName.Entity.Components);

            foreach (var component in entity.Components)
            {
                WriteComponent(jsonWriter, component);
            }

            jsonWriter.WriteEndArray();

            #endregion

            #region Children

            jsonWriter.WriteStartArray(PropertyName.Entity.Children);

            foreach (var childEntity in entity.Children)
            {
                WriteEntity(jsonWriter, childEntity);
            }

            jsonWriter.WriteEndArray();

            #endregion

            jsonWriter.WriteEndObject();
        }

        private void ReadEntity(JsonElement entityElement, Entity entity)
        {
            #region Name

            var name = entityElement.GetProperty(PropertyName.Entity.Name).GetString() ??
                       throw new InvalidOperationException($"Cannot deserialize scene. {PropertyName.Entity.Name} property of entity cannot be null.");
            entity.Name = name;

            #endregion

            #region Components

            var componentsElement = entityElement.GetProperty(PropertyName.Entity.Components).EnumerateArray();
            foreach (var componentElement in componentsElement)
            {
                ReadComponent(componentElement, entity);
            }

            #endregion

            #region Children

            var childrenElement = entityElement.GetProperty(PropertyName.Entity.Children).EnumerateArray();
            foreach (var childEntityElement in childrenElement)
            {
                var childEntity = new Entity();
                ReadEntity(childEntityElement, childEntity);
                entity.AddChild(childEntity);
            }

            #endregion
        }

        private void WriteComponent(Utf8JsonWriter jsonWriter, Component component)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyName.Component.ComponentId, component.ComponentId.Value);

            jsonWriter.WriteStartObject(PropertyName.Component.ComponentData);
            component.Serialize(new ComponentDataWriter(jsonWriter), _assetStore);
            jsonWriter.WriteEndObject();

            jsonWriter.WriteEndObject();
        }

        private void ReadComponent(JsonElement componentElement, Entity entity)
        {
            var componentIdString = componentElement.GetProperty(PropertyName.Component.ComponentId).GetString() ??
                                    throw new InvalidOperationException(
                                        $"Cannot deserialize scene. {PropertyName.Component.ComponentId} property of component cannot be null.");
            var componentId = new ComponentId(componentIdString);

            var componentDataElement = componentElement.GetProperty(PropertyName.Component.ComponentData);
            var component = _componentFactoryProvider.Get(componentId).Create();
            component.Deserialize(new ComponentDataReader(componentDataElement), _assetStore);

            entity.AddComponent(component);
        }

        private class ObjectWriter : IObjectWriter
        {
            private readonly Utf8JsonWriter _jsonWriter;

            protected ObjectWriter(Utf8JsonWriter jsonWriter)
            {
                _jsonWriter = jsonWriter;
            }

            public void WriteNull(string propertyName)
            {
                _jsonWriter.WriteNull(propertyName);
            }

            public void WriteBool(string propertyName, bool value)
            {
                _jsonWriter.WriteBoolean(propertyName, value);
            }

            public void WriteInt(string propertyName, int value)
            {
                _jsonWriter.WriteNumber(propertyName, value);
            }

            public void WriteDouble(string propertyName, double value)
            {
                _jsonWriter.WriteNumber(propertyName, value);
            }

            public void WriteString(string propertyName, string? value)
            {
                _jsonWriter.WriteString(propertyName, value);
            }

            public void WriteEnum<TEnum>(string propertyName, TEnum value) where TEnum : struct
            {
                _jsonWriter.WriteString(propertyName, value.ToString());
            }

            public void WriteVector2(string propertyName, Vector2 value)
            {
                _jsonWriter.WriteStartObject(propertyName);
                _jsonWriter.WriteNumber(PropertyName.Vector2.X, value.X);
                _jsonWriter.WriteNumber(PropertyName.Vector2.Y, value.Y);
                _jsonWriter.WriteEndObject();
            }

            public void WriteVector3(string propertyName, Vector3 value)
            {
                _jsonWriter.WriteStartObject(propertyName);
                _jsonWriter.WriteNumber(PropertyName.Vector3.X, value.X);
                _jsonWriter.WriteNumber(PropertyName.Vector3.Y, value.Y);
                _jsonWriter.WriteNumber(PropertyName.Vector3.Z, value.Z);
                _jsonWriter.WriteEndObject();
            }

            public void WriteAssetId(string propertyName, AssetId value)
            {
                _jsonWriter.WriteString(propertyName, value.Value);
            }

            public void WriteColor(string propertyName, Color value)
            {
                _jsonWriter.WriteNumber(propertyName, value.ToArgb());
            }

            public void WriteObject<T>(string propertyName, T value, Action<T, IObjectWriter> writeAction)
            {
                _jsonWriter.WriteStartObject(propertyName);
                writeAction(value, new ObjectWriter(_jsonWriter));
                _jsonWriter.WriteEndObject();
            }
        }

        private sealed class ComponentDataWriter : ObjectWriter, IComponentDataWriter
        {
            public ComponentDataWriter(Utf8JsonWriter jsonWriter) : base(jsonWriter)
            {
            }
        }

        private class ObjectReader : IObjectReader
        {
            private readonly JsonElement _jsonElement;

            protected ObjectReader(JsonElement jsonElement)
            {
                _jsonElement = jsonElement;
            }

            public bool IsDefined(string propertyName) => _jsonElement.TryGetProperty(propertyName, out _);
            public bool IsNull(string propertyName) => _jsonElement.GetProperty(propertyName).ValueKind == JsonValueKind.Null;
            public bool ReadBool(string propertyName) => _jsonElement.GetProperty(propertyName).GetBoolean();
            public int ReadInt(string propertyName) => _jsonElement.GetProperty(propertyName).GetInt32();
            public double ReadDouble(string propertyName) => _jsonElement.GetProperty(propertyName).GetDouble();
            public string? ReadString(string propertyName) => _jsonElement.GetProperty(propertyName).GetString();

            public TEnum ReadEnum<TEnum>(string propertyName) where TEnum : struct =>
                Enum.Parse<TEnum>(_jsonElement.GetProperty(propertyName).GetString());

            public Vector2 ReadVector2(string propertyName)
            {
                var vector2Element = _jsonElement.GetProperty(propertyName);
                var x = vector2Element.GetProperty(PropertyName.Vector2.X).GetDouble();
                var y = vector2Element.GetProperty(PropertyName.Vector2.Y).GetDouble();
                return new Vector2(x, y);
            }

            public Vector3 ReadVector3(string propertyName)
            {
                var vector3Element = _jsonElement.GetProperty(propertyName);
                var x = vector3Element.GetProperty(PropertyName.Vector3.X).GetDouble();
                var y = vector3Element.GetProperty(PropertyName.Vector3.Y).GetDouble();
                var z = vector3Element.GetProperty(PropertyName.Vector3.Z).GetDouble();
                return new Vector3(x, y, z);
            }

            public AssetId ReadAssetId(string propertyName) => new AssetId(_jsonElement.GetProperty(propertyName).GetGuid());
            public Color ReadColor(string propertyName) => Color.FromArgb(_jsonElement.GetProperty(propertyName).GetInt32());

            public T ReadObject<T>(string propertyName, Func<IObjectReader, T> readFunc) => readFunc(new ObjectReader(_jsonElement.GetProperty(propertyName)));
            public IEnumerable<string> EnumerateObject(string propertyName) => _jsonElement.GetProperty(propertyName).EnumerateObject().Select(p => p.Name);
        }

        private sealed class ComponentDataReader : ObjectReader, IComponentDataReader
        {
            public ComponentDataReader(JsonElement componentDataElement) : base(componentDataElement)
            {
            }
        }
    }
}