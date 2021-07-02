using System;
using System.Text.Json;
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
            var textureFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
            var textureFileContent = JsonSerializer.Deserialize<TextureFileContent>(textureFileJson);

            if (textureFileContent is null)
                throw new InvalidOperationException($"{nameof(TextureFileContent)} cannot be null.");
            if (textureFileContent.TextureFilePath == null)
                throw new InvalidOperationException($"{nameof(TextureFileContent)}.{nameof(TextureFileContent.TextureFilePath)} cannot be null.");

            var textureFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, textureFileContent.TextureFilePath);
            using var stream = _fileSystem.GetFile(textureFilePath).OpenRead();
            return _renderer2D.CreateTexture(stream);
        }

        protected override void UnloadAsset(ITexture asset)
        {
            asset.Dispose();
        }
    }
}