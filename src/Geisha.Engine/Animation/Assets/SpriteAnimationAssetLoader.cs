using System;
using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationAssetLoader : IAssetLoader
    {
        private readonly IFileSystem _fileSystem;

        public SpriteAnimationAssetLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public AssetType AssetType => AnimationAssetTypes.SpriteAnimation;
        public Type AssetClassType { get; } = typeof(SpriteAnimation);

        public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
        {
            using var fileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(fileStream);
            var spriteAnimationAssetContent = assetData.ReadJsonContent<SpriteAnimationAssetContent>();

            if (spriteAnimationAssetContent.Frames == null)
                throw new InvalidOperationException($"{nameof(SpriteAnimationAssetContent)}.{nameof(SpriteAnimationAssetContent.Frames)} cannot be null.");

            var frames = spriteAnimationAssetContent.Frames.Select(f =>
            {
                var sprite = assetStore.GetAsset<Sprite>(new AssetId(f.SpriteAssetId));
                return new SpriteAnimationFrame(sprite, f.Duration);
            }).ToArray();

            return new SpriteAnimation(frames, TimeSpan.FromTicks(spriteAnimationAssetContent.DurationTicks));
        }

        public void UnloadAsset(object asset)
        {
        }
    }
}