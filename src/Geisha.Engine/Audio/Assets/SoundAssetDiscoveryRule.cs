using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private readonly IJsonSerializer _jsonSerializer;

        public SoundAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == AudioFileExtensions.Sound)
            {
                var soundFileContent = _jsonSerializer.Deserialize<SoundFileContent>(file.ReadAllText());
                var assetInfo = new AssetInfo(
                    new AssetId(soundFileContent.AssetId),
                    typeof(ISound),
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