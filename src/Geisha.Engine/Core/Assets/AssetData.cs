using System;
using System.IO;
using System.Text.Json;

namespace Geisha.Engine.Core.Assets
{
    public sealed class AssetData
    {
        private readonly Content _content;

        public enum AssetContentType
        {
            Binary,
            String,
            Json
        }

        public static AssetData CreateWithBinaryContent(AssetId assetId, AssetType assetType, Stream content)
        {
            return new AssetData(assetId, assetType, new BinaryContent(content));
        }

        public static AssetData CreateWithStringContent(AssetId assetId, AssetType assetType, string content)
        {
            return new AssetData(assetId, assetType, new StringContent(content));
        }

        public static AssetData CreateWithJsonContent<T>(AssetId assetId, AssetType assetType, T content) where T : notnull
        {
            return new AssetData(assetId, assetType, new JsonContent(content));
        }

        public static AssetData Load(string filePath)
        {
            throw new NotImplementedException();
        }

        public static AssetData Load(Stream stream)
        {
            using var jsonDocument = JsonDocument.Parse(stream);
            var rootElement = jsonDocument.RootElement;
            var assetId = new AssetId(rootElement.GetProperty("AssetId").GetGuid());
            var assetTypeString = rootElement.GetProperty("AssetType").GetString() ?? throw new InvalidOperationException("AssetType cannot be null.");
            var assetType = new AssetType(assetTypeString);
            var contentTypeString = rootElement.GetProperty("ContentType").GetString() ?? throw new InvalidOperationException("ContentType cannot be null.");
            var contentType = Enum.Parse<AssetContentType>(contentTypeString);
            var contentProperty = rootElement.GetProperty("Content");
            Content content = contentType switch
            {
                AssetContentType.Binary => new BinaryContent(contentProperty),
                AssetContentType.String => new StringContent(contentProperty),
                AssetContentType.Json => new JsonContent(contentProperty),
                _ => throw new ArgumentOutOfRangeException("ContentType", contentType, "Unknown content type.")
            };

            return new AssetData(assetId, assetType, content);
        }

        public AssetId AssetId { get; }
        public AssetType AssetType { get; }
        public AssetContentType ContentType => _content.Type;

        public Stream ReadBinaryContent()
        {
            return _content.ReadBinary();
        }

        public string ReadStringContent()
        {
            return _content.ReadString();
        }

        public T ReadJsonContent<T>() where T : notnull
        {
            return _content.ReadJson<T>();
        }

        public void Save(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            using var jsonWriter = new Utf8JsonWriter(stream);
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("AssetId", AssetId.Value);
            jsonWriter.WriteString("AssetType", AssetType.Value);
            jsonWriter.WriteString("ContentType", ContentType.ToString());
            jsonWriter.WritePropertyName("Content");
            _content.WriteTo(jsonWriter);
            jsonWriter.WriteEndObject();
        }

        private AssetData(AssetId assetId, AssetType assetType, Content content)
        {
            AssetId = assetId;
            AssetType = assetType;
            _content = content;
        }

        private abstract class Content
        {
            public abstract AssetContentType Type { get; }
            public abstract void WriteTo(Utf8JsonWriter jsonWriter);
            public virtual Stream ReadBinary() => throw new InvalidOperationException($"{nameof(AssetData)} has no binary content.");
            public virtual string ReadString() => throw new InvalidOperationException($"{nameof(AssetData)} has no string content.");
            public virtual T ReadJson<T>() where T : notnull => throw new InvalidOperationException($"{nameof(AssetData)} has no JSON content.");
        }

        private sealed class BinaryContent : Content
        {
            private readonly MemoryStream _content;

            public BinaryContent(Stream content)
            {
                _content = new MemoryStream();
                content.CopyTo(_content);
            }

            public BinaryContent(JsonElement jsonElement)
            {
                _content = new MemoryStream(jsonElement.GetBytesFromBase64());
            }

            public override AssetContentType Type => AssetContentType.Binary;

            public override void WriteTo(Utf8JsonWriter jsonWriter)
            {
                jsonWriter.WriteBase64StringValue(_content.GetBuffer().AsSpan(0, (int) _content.Length));
            }

            public override Stream ReadBinary()
            {
                _content.Position = 0;
                return _content;
            }
        }

        private sealed class StringContent : Content
        {
            private readonly string _content;

            public StringContent(string content)
            {
                _content = content;
            }

            public StringContent(JsonElement jsonElement)
            {
                _content = jsonElement.GetString() ?? throw new InvalidOperationException("String content cannot be null.");
            }

            public override AssetContentType Type => AssetContentType.String;

            public override void WriteTo(Utf8JsonWriter jsonWriter)
            {
                jsonWriter.WriteStringValue(_content);
            }

            public override string ReadString() => _content;
        }

        private sealed class JsonContent : Content
        {
            private readonly string _content;

            public JsonContent(object content)
            {
                _content = JsonSerializer.Serialize(content);
            }

            public JsonContent(JsonElement jsonElement)
            {
                _content = jsonElement.GetRawText();
            }

            public override AssetContentType Type => AssetContentType.Json;

            public override void WriteTo(Utf8JsonWriter jsonWriter)
            {
                using var jsonDocument = JsonDocument.Parse(_content);
                jsonDocument.RootElement.WriteTo(jsonWriter);
            }

            public override T ReadJson<T>() => JsonSerializer.Deserialize<T>(_content) ?? throw new InvalidOperationException("JSON content cannot be null.");
        }
    }
}