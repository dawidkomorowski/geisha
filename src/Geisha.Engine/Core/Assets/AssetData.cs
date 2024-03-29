﻿using System;
using System.IO;
using System.Text.Json;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Represents an asset in the form of asset metadata and actual asset content. It also provides an API to load and
    ///     save asset data to and from stream or file.
    /// </summary>
    public sealed class AssetData
    {
        /// <summary>
        ///     Defines format of asset content stored in <see cref="AssetData" />.
        /// </summary>
        public enum AssetContentFormat
        {
            /// <summary>
            ///     Specifies that format of asset content is binary.
            /// </summary>
            Binary,

            /// <summary>
            ///     Specifies that format of asset content is <see cref="string" />.
            /// </summary>
            String,

            /// <summary>
            ///     Specifies that format of asset content is JSON.
            /// </summary>
            Json
        }

        private readonly Content _content;

        private AssetData(AssetId assetId, AssetType assetType, Content content)
        {
            AssetId = assetId;
            AssetType = assetType;
            _content = content;
        }

        /// <summary>
        ///     Gets <see cref="Assets.AssetId" /> of this <see cref="AssetData" />.
        /// </summary>
        public AssetId AssetId { get; }

        /// <summary>
        ///     Gets <see cref="Assets.AssetType" /> of this <see cref="AssetData" />.
        /// </summary>
        public AssetType AssetType { get; }

        /// <summary>
        ///     Gets <see cref="AssetContentFormat" /> of this <see cref="AssetData" />.
        /// </summary>
        public AssetContentFormat ContentFormat => _content.Format;

        /// <summary>
        ///     Creates new instance of <see cref="AssetData" /> with specified <see cref="Assets.AssetId" />,
        ///     <see cref="Assets.AssetType" /> and content in binary format.
        /// </summary>
        /// <param name="assetId"><see cref="Assets.AssetId" /> of <see cref="AssetData" />.</param>
        /// <param name="assetType"><see cref="Assets.AssetType" /> of <see cref="AssetData" />.</param>
        /// <param name="content"><see cref="Stream" /> to read from the binary content of <see cref="AssetData" />.</param>
        /// <returns>New instance of <see cref="AssetData" /> with binary content.</returns>
        public static AssetData CreateWithBinaryContent(AssetId assetId, AssetType assetType, Stream content) =>
            new AssetData(assetId, assetType, new BinaryContent(content));

        /// <summary>
        ///     Creates new instance of <see cref="AssetData" /> with specified <see cref="Assets.AssetId" />,
        ///     <see cref="Assets.AssetType" /> and content in <see cref="string" /> format.
        /// </summary>
        /// <param name="assetId"><see cref="Assets.AssetId" /> of <see cref="AssetData" />.</param>
        /// <param name="assetType"><see cref="Assets.AssetType" /> of <see cref="AssetData" />.</param>
        /// <param name="content"><see cref="string" /> to be used as content of <see cref="AssetData" />.</param>
        /// <returns>New instance of <see cref="AssetData" /> with <see cref="string" /> content.</returns>
        public static AssetData CreateWithStringContent(AssetId assetId, AssetType assetType, string content) =>
            new AssetData(assetId, assetType, new StringContent(content));

        /// <summary>
        ///     Creates new instance of <see cref="AssetData" /> with specified <see cref="Assets.AssetId" />,
        ///     <see cref="Assets.AssetType" /> and content in JSON format.
        /// </summary>
        /// <param name="assetId"><see cref="Assets.AssetId" /> of <see cref="AssetData" />.</param>
        /// <param name="assetType"><see cref="Assets.AssetType" /> of <see cref="AssetData" />.</param>
        /// <param name="content">
        ///     Instance of <typeparamref name="T" /> to be serialized as JSON content of
        ///     <see cref="AssetData" />.
        /// </param>
        /// <returns>New instance of <see cref="AssetData" /> with <see cref="string" /> content.</returns>
        public static AssetData CreateWithJsonContent<T>(AssetId assetId, AssetType assetType, T content) where T : notnull =>
            new AssetData(assetId, assetType, new JsonContent(content));

        /// <summary>
        ///     Loads <see cref="AssetData" /> from specified file.
        /// </summary>
        /// <param name="filePath">Path to file containing asset data.</param>
        /// <returns>New instance of <see cref="AssetData" /> loaded from specified file.</returns>
        public static AssetData Load(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            return Load(fileStream);
        }

        /// <summary>
        ///     Loads <see cref="AssetData" /> from specified stream.
        /// </summary>
        /// <param name="stream">Stream containing asset data.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"><c>AssetType</c> property is null or <c>ContentFormat</c> property is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <c>ContentFormat</c> property value is none of available formats
        ///     <see cref="AssetContentFormat" />.
        /// </exception>
        public static AssetData Load(Stream stream)
        {
            using var jsonDocument = JsonDocument.Parse(stream);
            var rootElement = jsonDocument.RootElement;
            var assetId = new AssetId(rootElement.GetProperty("AssetId").GetGuid());
            var assetTypeString = rootElement.GetProperty("AssetType").GetString() ?? throw new InvalidOperationException("AssetType cannot be null.");
            var assetType = new AssetType(assetTypeString);
            var contentFormatString = rootElement.GetProperty("ContentFormat").GetString() ??
                                      throw new InvalidOperationException("ContentFormat cannot be null.");
            var contentFormat = Enum.Parse<AssetContentFormat>(contentFormatString);
            var contentProperty = rootElement.GetProperty("Content");
            Content content = contentFormat switch
            {
                AssetContentFormat.Binary => new BinaryContent(contentProperty),
                AssetContentFormat.String => new StringContent(contentProperty),
                AssetContentFormat.Json => new JsonContent(contentProperty),
                _ => throw new ArgumentOutOfRangeException("ContentFormat", contentFormat, "Unknown content format.")
            };

            return new AssetData(assetId, assetType, content);
        }

        /// <summary>
        ///     Returns <see cref="Stream" /> for reading binary content.
        /// </summary>
        /// <returns><see cref="Stream" /> for reading binary content.</returns>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="ContentFormat" /> is not <see cref="AssetContentFormat.Binary" />.
        /// </exception>
        public Stream ReadBinaryContent() => _content.ReadBinary();

        /// <summary>
        ///     Returns <see cref="string" /> content.
        /// </summary>
        /// <returns><see cref="string" /> content.</returns>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="ContentFormat" /> is not <see cref="AssetContentFormat.String" />.
        /// </exception>
        public string ReadStringContent() => _content.ReadString();

        /// <summary>
        ///     Returns instance of <typeparamref name="T" /> deserialized from JSON content.
        /// </summary>
        /// <typeparam name="T">Type to be used for deserializing JSON content.</typeparam>
        /// <returns>Instance of <typeparamref name="T" /> deserialized from JSON content.</returns>
        /// <exception cref="InvalidOperationException"><see cref="ContentFormat" /> is not <see cref="AssetContentFormat.Json" />.</exception>
        public T ReadJsonContent<T>() where T : notnull => _content.ReadJson<T>();

        /// <summary>
        ///     Saves <see cref="AssetData" /> to specified file.
        /// </summary>
        /// <param name="filePath">Path to file where to save asset data.</param>
        public void Save(string filePath)
        {
            using var fileStream = File.Open(filePath, FileMode.Create);
            Save(fileStream);
        }

        /// <summary>
        ///     Saves <see cref="AssetData" /> to specified stream.
        /// </summary>
        /// <param name="stream">Stream where to save asset data.</param>
        public void Save(Stream stream)
        {
            using var jsonWriter = new Utf8JsonWriter(stream);
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("AssetId", AssetId.Value);
            jsonWriter.WriteString("AssetType", AssetType.Value);
            jsonWriter.WriteString("ContentFormat", ContentFormat.ToString());
            jsonWriter.WritePropertyName("Content");
            _content.WriteTo(jsonWriter);
            jsonWriter.WriteEndObject();
        }

        private abstract class Content
        {
            public abstract AssetContentFormat Format { get; }
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

            public override AssetContentFormat Format => AssetContentFormat.Binary;

            public override void WriteTo(Utf8JsonWriter jsonWriter)
            {
                jsonWriter.WriteBase64StringValue(_content.GetBuffer().AsSpan(0, (int)_content.Length));
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

            public override AssetContentFormat Format => AssetContentFormat.String;

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

            public override AssetContentFormat Format => AssetContentFormat.Json;

            public override void WriteTo(Utf8JsonWriter jsonWriter)
            {
                using var jsonDocument = JsonDocument.Parse(_content);
                jsonDocument.RootElement.WriteTo(jsonWriter);
            }

            public override T ReadJson<T>() => JsonSerializer.Deserialize<T>(_content) ?? throw new InvalidOperationException("JSON content cannot be null.");
        }
    }
}