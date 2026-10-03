using System;
using System.Collections.Immutable;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
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

            var frames = ImmutableArray.CreateBuilder<SpriteAnimationFrame>();

            foreach (var frame in spriteAnimationAssetContent.Frames)
            {
                var sprite = assetStore.GetAsset<Sprite>(new AssetId(frame.SpriteAssetId));
                frames.Add(new SpriteAnimationFrame(sprite, frame.Duration));
            }

            return new SpriteAnimation(frames.ToImmutable(), TimeSpan.FromTicks(spriteAnimationAssetContent.DurationTicks));
        }

        public void UnloadAsset(object asset)
        {
        }
    }
}