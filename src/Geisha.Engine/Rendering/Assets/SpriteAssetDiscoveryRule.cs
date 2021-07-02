using System;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class SpriteAssetDiscoveryRule : IAssetDiscoveryRule
    {
        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == RenderingFileExtensions.Sprite)
            {
                var spriteFileContent = JsonSerializer.Deserialize<SpriteFileContent>(file.ReadAllText());

                if (spriteFileContent is null) throw new ArgumentException($"Cannot parse {nameof(SpriteFileContent)} from file {file.Path}.");

                var assetInfo = new AssetInfo(
                    new AssetId(spriteFileContent.AssetId),
                    typeof(Sprite),
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