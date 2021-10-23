using System;
using Geisha.Common.FileSystem;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Handle assets dependencies (texture should not be unloaded as long as sprite is not unloaded).
    internal sealed class SpriteAssetLoader : IAssetLoader
    {
        private readonly IFileSystem _fileSystem;

        public SpriteAssetLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public AssetType AssetType => RenderingAssetTypes.Sprite;
        public Type AssetClassType { get; } = typeof(Sprite);

        public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
        {
            var fileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(fileStream);
            var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();

            var textureAssetId = new AssetId(spriteAssetContent.TextureAssetId);

            return new Sprite(assetStore.GetAsset<ITexture>(textureAssetId))
            {
                SourceUV = SerializableVector2.ToVector2(spriteAssetContent.SourceUV),
                SourceDimension = SerializableVector2.ToVector2(spriteAssetContent.SourceDimension),
                SourceAnchor = SerializableVector2.ToVector2(spriteAssetContent.SourceAnchor),
                PixelsPerUnit = spriteAssetContent.PixelsPerUnit
            };
        }

        public void UnloadAsset(object asset)
        {
        }
    }
}