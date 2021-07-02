using System;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private const string InputMappingFileExtension = ".input";

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == InputMappingFileExtension)
            {
                var inputMappingFileContent = JsonSerializer.Deserialize<InputMappingFileContent>(file.ReadAllText());

                if (inputMappingFileContent is null) throw new ArgumentException($"Cannot parse {nameof(InputMappingFileContent)} from file {file.Path}.");

                var assetInfo = new AssetInfo(
                    new AssetId(inputMappingFileContent.AssetId),
                    typeof(InputMapping),
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