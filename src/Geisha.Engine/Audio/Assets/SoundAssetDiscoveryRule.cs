using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private const string SoundFileExtension = ".sound";
        private readonly IJsonSerializer _jsonSerializer;

        public SoundAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == SoundFileExtension)
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