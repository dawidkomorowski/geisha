using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class SpriteAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private const string SpriteFileExtension = ".sprite";
        private readonly IJsonSerializer _jsonSerializer;

        public SpriteAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == SpriteFileExtension)
            {
                var spriteFileContent = _jsonSerializer.Deserialize<SpriteFileContent>(file.ReadAllText());
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