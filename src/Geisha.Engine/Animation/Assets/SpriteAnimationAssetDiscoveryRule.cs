using System;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationAssetDiscoveryRule : IAssetDiscoveryRule
    {
        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == AnimationFileExtensions.SpriteAnimation)
            {
                var spriteAnimationFileContent = JsonSerializer.Deserialize<SpriteAnimationFileContent>(file.ReadAllText());

                if (spriteAnimationFileContent is null)
                    throw new ArgumentException($"Cannot parse {nameof(SpriteAnimationFileContent)} from file {file.Path}.");

                var assetInfo = new AssetInfo(
                    new AssetId(spriteAnimationFileContent.AssetId),
                    typeof(SpriteAnimation),
                    file.Path);
                return SingleOrEmpty.Single(assetInfo);
            }
            else
            {
                return SingleOrEmpty.Empty<AssetInfo>();
            }
        }
    }
}