using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureManagedAsset : ManagedAsset<ITexture>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IRenderer2D _renderer2D;

        public TextureManagedAsset(AssetInfo assetInfo, IRenderer2D renderer2D, IFileSystem fileSystem, IJsonSerializer jsonSerializer) : base(assetInfo)
        {
            _renderer2D = renderer2D;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override ITexture LoadAsset()
        {
            var textureFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
            var textureFileContent = _jsonSerializer.Deserialize<TextureFileContent>(textureFileJson);

            var textureFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, textureFileContent.TextureFilePath);
            using (var stream = _fileSystem.GetFile(textureFilePath).OpenRead())
            {
                return _renderer2D.CreateTexture(stream);
            }
        }

        protected override void UnloadAsset(ITexture asset)
        {
            asset.Dispose();
        }
    }
}