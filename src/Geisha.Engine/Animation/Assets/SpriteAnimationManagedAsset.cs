using System;
using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationManagedAsset : ManagedAsset<SpriteAnimation>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IAssetStore _assetStore;

        public SpriteAnimationManagedAsset(AssetInfo assetInfo, IFileSystem fileSystem, IJsonSerializer jsonSerializer, IAssetStore assetStore) :
            base(assetInfo)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
            _assetStore = assetStore;
        }

        protected override SpriteAnimation LoadAsset()
        {
            var spriteAnimationFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
            var spriteAnimationFileContent = _jsonSerializer.Deserialize<SpriteAnimationFileContent>(spriteAnimationFileJson);

            if (spriteAnimationFileContent.Frames == null)
            {
                throw new InvalidOperationException($"{nameof(SpriteAnimationFileContent)}.{nameof(SpriteAnimationFileContent.Frames)} cannot be null.");
            }

            var frames = spriteAnimationFileContent.Frames.Select(f =>
            {
                var sprite = _assetStore.GetAsset<Sprite>(new AssetId(f.SpriteAssetId));
                return new SpriteAnimationFrame(sprite, f.Duration);
            });

            return new SpriteAnimation(frames, TimeSpan.FromTicks(spriteAnimationFileContent.DurationTicks));
        }

        protected override void UnloadAsset(SpriteAnimation asset)
        {
        }
    }
}