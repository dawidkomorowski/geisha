using System;
using System.Linq;
using System.Text.Json;
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
            var spriteAnimationFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
            var spriteAnimationFileContent = JsonSerializer.Deserialize<SpriteAnimationFileContent>(spriteAnimationFileJson);

            if (spriteAnimationFileContent is null)
            {
                throw new InvalidOperationException($"{nameof(SpriteAnimationFileContent)} cannot be null.");
            }

            if (spriteAnimationFileContent.Frames == null)
            {
                throw new InvalidOperationException($"{nameof(SpriteAnimationFileContent)}.{nameof(SpriteAnimationFileContent.Frames)} cannot be null.");
            }

            var frames = spriteAnimationFileContent.Frames.Select(f =>
            {
                var sprite = _assetStore.GetAsset<Sprite>(new AssetId(f.SpriteAssetId));
                return new SpriteAnimationFrame(sprite, f.Duration);
            }).ToArray();

            return new SpriteAnimation(frames, TimeSpan.FromTicks(spriteAnimationFileContent.DurationTicks));
        }

        protected override void UnloadAsset(SpriteAnimation asset)
        {
        }
    }
}