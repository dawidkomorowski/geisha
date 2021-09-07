using System;
using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationManagedAsset : ManagedAsset<SpriteAnimation>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAssetStore _assetStore;

        public SpriteAnimationManagedAsset(AssetInfo assetInfo, IFileSystem fileSystem, IAssetStore assetStore) :
            base(assetInfo)
        {
            _fileSystem = fileSystem;
            _assetStore = assetStore;
        }

        protected override SpriteAnimation LoadAsset()
        {
            using var fileStream = _fileSystem.GetFile(AssetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(fileStream);
            var spriteAnimationAssetContent = assetData.ReadJsonContent<SpriteAnimationAssetContent>();

            if (spriteAnimationAssetContent.Frames == null)
            {
                throw new InvalidOperationException($"{nameof(SpriteAnimationAssetContent)}.{nameof(SpriteAnimationAssetContent.Frames)} cannot be null.");
            }

            var frames = spriteAnimationAssetContent.Frames.Select(f =>
            {
                var sprite = _assetStore.GetAsset<Sprite>(new AssetId(f.SpriteAssetId));
                return new SpriteAnimationFrame(sprite, f.Duration);
            }).ToArray();

            return new SpriteAnimation(frames, TimeSpan.FromTicks(spriteAnimationAssetContent.DurationTicks));
        }

        protected override void UnloadAsset(SpriteAnimation asset)
        {
        }
    }
}