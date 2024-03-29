﻿using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureAssetLoader : IAssetLoader
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRenderingContext2D _renderingContext2D;

        public TextureAssetLoader(IRenderingBackend renderingBackend, IFileSystem fileSystem)
        {
            _renderingContext2D = renderingBackend.Context2D;
            _fileSystem = fileSystem;
        }

        public AssetType AssetType => RenderingAssetTypes.Texture;
        public Type AssetClassType { get; } = typeof(ITexture);

        public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
        {
            using var assetFileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(assetFileStream);
            var textureAssetContent = assetData.ReadJsonContent<TextureAssetContent>();

            if (textureAssetContent.TextureFilePath == null)
                throw new InvalidOperationException($"{nameof(TextureAssetContent)}.{nameof(TextureAssetContent.TextureFilePath)} cannot be null.");

            var textureFilePath = PathUtils.GetSiblingPath(assetInfo.AssetFilePath, textureAssetContent.TextureFilePath);
            using var textureFileStream = _fileSystem.GetFile(textureFilePath).OpenRead();
            return _renderingContext2D.CreateTexture(textureFileStream);
        }

        public void UnloadAsset(object asset)
        {
            var texture = (ITexture)asset;
            texture.Dispose();
        }
    }
}