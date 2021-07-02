using System;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundAssetDiscoveryRule : IAssetDiscoveryRule
    {
        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == AudioFileExtensions.Sound)
            {
                var soundFileContent = JsonSerializer.Deserialize<SoundFileContent>(file.ReadAllText());

                if (soundFileContent is null) throw new ArgumentException($"Cannot parse {nameof(SoundFileContent)} from file {file.Path}.");

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