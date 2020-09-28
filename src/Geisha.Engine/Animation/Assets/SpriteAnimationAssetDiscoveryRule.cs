using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private readonly IJsonSerializer _jsonSerializer;

        public SpriteAnimationAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == AnimationFileExtensions.SpriteAnimation)
            {
                var spriteFileContent = _jsonSerializer.Deserialize<SpriteAnimationFileContent>(file.ReadAllText());
                var assetInfo = new AssetInfo(
                    new AssetId(spriteFileContent.AssetId),
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