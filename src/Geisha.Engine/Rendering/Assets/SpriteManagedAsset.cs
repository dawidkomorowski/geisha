using Geisha.Common.FileSystem;
using Geisha.Common.Math.Serialization;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Handle assets dependencies (texture should not be unloaded as long as sprite is not unloaded).
    internal sealed class SpriteManagedAsset : ManagedAsset<Sprite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IAssetStore _assetStore;

        public SpriteManagedAsset(AssetInfo assetInfo, IFileSystem fileSystem, IJsonSerializer jsonSerializer, IAssetStore assetStore) : base(assetInfo)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
            _assetStore = assetStore;
        }

        protected override Sprite LoadAsset()
        {
            var spriteFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
            var spriteFileContent = _jsonSerializer.Deserialize<SpriteFileContent>(spriteFileJson);
            var textureAssetId = new AssetId(spriteFileContent.TextureAssetId);

            return new Sprite(_assetStore.GetAsset<ITexture>(textureAssetId))
            {
                SourceUV = SerializableVector2.ToVector2(spriteFileContent.SourceUV),
                SourceDimension = SerializableVector2.ToVector2(spriteFileContent.SourceDimension),
                SourceAnchor = SerializableVector2.ToVector2(spriteFileContent.SourceAnchor),
                PixelsPerUnit = spriteFileContent.PixelsPerUnit
            };
        }

        protected override void UnloadAsset(Sprite asset)
        {
        }
    }
}