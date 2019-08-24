using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private const string TextureFileExtension = ".texture";
        private readonly IJsonSerializer _jsonSerializer;

        public TextureAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == TextureFileExtension)
            {
                var textureFileContent = _jsonSerializer.Deserialize<TextureFileContent>(file.ReadAllText());
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