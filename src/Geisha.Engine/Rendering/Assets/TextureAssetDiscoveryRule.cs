using System;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureAssetDiscoveryRule : IAssetDiscoveryRule
    {
        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == RenderingFileExtensions.Texture)
            {
                var textureFileContent = JsonSerializer.Deserialize<TextureFileContent>(file.ReadAllText());

                if (textureFileContent is null) throw new ArgumentException($"Cannot parse {nameof(TextureFileContent)} from file {file.Path}.");

                var assetInfo = new AssetInfo(
                    new AssetId(textureFileContent.AssetId),
                    typeof(ITexture),
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