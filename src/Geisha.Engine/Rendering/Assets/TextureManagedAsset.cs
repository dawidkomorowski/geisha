using System;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureManagedAsset : ManagedAsset<ITexture>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRenderer2D _renderer2D;

        public TextureManagedAsset(AssetInfo assetInfo, IRenderer2D renderer2D, IFileSystem fileSystem) : base(assetInfo)
        {
            _renderer2D = renderer2D;
            _fileSystem = fileSystem;
        }

        protected override ITexture LoadAsset()
        {
            using var assetFileStream = _fileSystem.GetFile(AssetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(assetFileStream);
            var textureFileContent = assetData.ReadJsonContent<TextureFileContent>();

            if (textureFileContent.TextureFilePath == null)
                throw new InvalidOperationException($"{nameof(TextureFileContent)}.{nameof(TextureFileContent.TextureFilePath)} cannot be null.");

            var textureFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, textureFileContent.TextureFilePath);
            using var textureFileStream = _fileSystem.GetFile(textureFilePath).OpenRead();
            return _renderer2D.CreateTexture(textureFileStream);
        }

        protected override void UnloadAsset(ITexture asset)
        {
            asset.Dispose();
        }
    }
}