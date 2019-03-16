using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    public sealed class InputMappingAssetDiscoveryRule : IAssetDiscoveryRule
    {
        private const string InputMappingFileExtension = ".input";
        private readonly IJsonSerializer _jsonSerializer;

        public InputMappingAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == InputMappingFileExtension)
            {
                var inputMappingFileContent = _jsonSerializer.Deserialize<InputMappingFileContent>(file.ReadAllText());
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