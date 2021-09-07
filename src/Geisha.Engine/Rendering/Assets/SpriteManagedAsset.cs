using Geisha.Common.FileSystem;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Handle assets dependencies (texture should not be unloaded as long as sprite is not unloaded).
    internal sealed class SpriteManagedAsset : ManagedAsset<Sprite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAssetStore _assetStore;

        public SpriteManagedAsset(AssetInfo assetInfo, IFileSystem fileSystem, IAssetStore assetStore) : base(assetInfo)
        {
            _fileSystem = fileSystem;
            _assetStore = assetStore;
        }

        protected override Sprite LoadAsset()
        {
            var fileStream = _fileSystem.GetFile(AssetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(fileStream);
            var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();

            var textureAssetId = new AssetId(spriteAssetContent.TextureAssetId);

            return new Sprite(_assetStore.GetAsset<ITexture>(textureAssetId))
            {
                SourceUV = SerializableVector2.ToVector2(spriteAssetContent.SourceUV),
                SourceDimension = SerializableVector2.ToVector2(spriteAssetContent.SourceDimension),
                SourceAnchor = SerializableVector2.ToVector2(spriteAssetContent.SourceAnchor),
                PixelsPerUnit = spriteAssetContent.PixelsPerUnit
            };
        }

        protected override void UnloadAsset(Sprite asset)
        {
        }
    }
}